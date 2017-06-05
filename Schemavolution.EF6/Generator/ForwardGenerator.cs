using Schemavolution.Specification.Implementation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;

namespace Schemavolution.EF6.Generator
{
    class ForwardGenerator : IGraphVisitor
    {
        private readonly string _databaseName;

        private MigrationDelta _difference;
        private ImmutableList<string> _sql = ImmutableList<string>.Empty;
        private ImmutableStack<Migration> _working = ImmutableStack<Migration>.Empty;

        public ForwardGenerator(string databaseName, MigrationDelta difference)
        {
            _databaseName = databaseName;
            _difference = difference;
        }

        public bool Any => _difference.Any;
        public Migration Head => _difference.Head;
        public ImmutableList<string> Sql => _sql;

        public bool AddMigration(Migration migration)
        {
            if (_working.Contains(migration))
                return false;

            foreach (var prerequisite in migration.AllPrerequisites
                .Where(p => _difference.Contains(p)))
            {
                if (!AddMigration(prerequisite))
                    return false;
            }

            _working = _working.Push(migration);

            var migrationsAffected = new MigrationHistoryBuilder();
            migrationsAffected.Append(migration);
            string[] result = migration.GenerateSql(migrationsAffected, this);
            _sql = _sql.AddRange(result);
            var mementos = migrationsAffected.MigrationHistory.GetMementos().ToList();
            _sql = _sql.Add(GenerateInsertStatement(_databaseName, mementos));
            if (mementos.SelectMany(m => m.Prerequisites).SelectMany(p => p.Value).Any())
                _sql = _sql.Add(GeneratePrerequisiteInsertStatements(_databaseName, mementos));
            _difference = _difference.Subtract(migrationsAffected.MigrationHistory);

            _working = _working.Pop();

            return true;
        }

        public ImmutableList<Migration> PullPrerequisitesForward(Migration migration, Migration origin, Func<Migration, bool> canOptimize)
        {
            ImmutableList<Migration> optimizableMigrations =
                ImmutableList<Migration>.Empty;

            foreach (var prerequisite in migration.AllPrerequisites
                .Where(p => p != origin)
                .Where(p => _difference.Contains(p)))
            {
                if (Follows(prerequisite, origin))
                {
                    if (canOptimize(prerequisite))
                    {
                        optimizableMigrations = optimizableMigrations.Add(prerequisite);
                    }
                    else
                    {
                        return ImmutableList<Migration>.Empty;
                    }
                }
                else
                {
                    if (!AddMigration(prerequisite))
                    {
                        return ImmutableList<Migration>.Empty;
                    }
                }
            }

            optimizableMigrations = optimizableMigrations.Add(migration);
            return optimizableMigrations;
        }

        private bool Follows(Migration migration, Migration origin)
        {
            return migration.AllPrerequisites
                .Any(p => p == origin || Follows(p, origin));
        }

        private string GenerateInsertStatement(string databaseName, IEnumerable<MigrationMemento> migrations)
        {
            string[] values = migrations.Select(migration => GenerateMigrationValue(migration)).ToArray();
            var insert = $@"INSERT INTO [{databaseName}].[dbo].[__MergableMigrationHistory]
    ([Type], [HashCode], [Attributes])
    VALUES{String.Join(",", values)}";
            return insert;
        }

        private string GenerateMigrationValue(MigrationMemento migration)
        {
            string attributes = JsonConvert.SerializeObject(migration.Attributes);
            string hex = $"0x{migration.HashCode.ToString("X")}";
            return $@"
    ('{migration.Type}', {hex}, '{attributes.Replace("'", "''")}')";
        }

        private string GeneratePrerequisiteInsertStatements(string databaseName, IEnumerable<MigrationMemento> migrations)
        {
            var joins =
                from migration in migrations
                from role in migration.Prerequisites
                from prerequisite in role.Value
                select new { MigrationHashCode = migration.HashCode, Role = role.Key, PrerequisiteHashCode = prerequisite };
            string[] values = joins.Select(join => GeneratePrerequisiteSelect(databaseName, join.MigrationHashCode, join.Role, join.PrerequisiteHashCode)).ToArray();
            string sql = $@"INSERT INTO [{databaseName}].[dbo].[__MergableMigrationHistoryPrerequisite]
    ([MigrationId], [Role], [PrerequisiteMigrationId]){string.Join(@"
UNION ALL", values)}";
            return sql;
        }

        string GeneratePrerequisiteSelect(string databaseName, BigInteger migrationHashCode, string role, BigInteger prerequisiteHashCode)
        {
            return $@"
SELECT m.MigrationId, '{role}', p.MigrationId
FROM [{databaseName}].[dbo].[__MergableMigrationHistory] m,
     [{databaseName}].[dbo].[__MergableMigrationHistory] p
WHERE m.HashCode = 0x{migrationHashCode.ToString("X")} AND p.HashCode = 0x{prerequisiteHashCode.ToString("X")}";
        }
    }
}
