using MergableMigrations.Specification.Implementation;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace MergableMigrations.EF6.Generator
{
    class RollbackGenerator
    {
        private readonly string _databaseName;

        private ImmutableList<string> _sql = ImmutableList<string>.Empty;
        private MigrationDelta _ahead;

        public RollbackGenerator(string databaseName, MigrationDelta ahead)
        {
            _databaseName = databaseName;
            _ahead = ahead;
        }

        public bool Any => _ahead.Any;
        public Migration Head => _ahead.Head;
        public ImmutableList<string> Sql => _sql;

        public void AddMigration(Migration migration)
        {
            var migrationsAffected = new MigrationHistoryBuilder();
            migrationsAffected.Append(migration);
            string[] rollbackSql = migration.GenerateRollbackSql(migrationsAffected);
            var mementos = migrationsAffected.MigrationHistory.GetMementos().ToList();
            string[] deleteStatements = GenerateDeleteStatements(_databaseName, mementos);
            _sql = _sql.InsertRange(0, deleteStatements);
            _sql = _sql.InsertRange(0, rollbackSql);
            _ahead = _ahead.Subtract(migrationsAffected.MigrationHistory);
        }

        private string[] GenerateDeleteStatements(string databaseName, IEnumerable<MigrationMemento> migrations)
        {
            var hashCodes = string.Join(", ", migrations.Select(m => $"0x{m.HashCode.ToString("X")}"));
            string[] sql =
            {
                $@"DELETE p
FROM [{databaseName}].[dbo].[__MergableMigrationHistory] m
JOIN [{databaseName}].[dbo].[__MergableMigrationHistoryPrerequisite] p
  ON p.MigrationId = m.MigrationId
WHERE m.HashCode IN ({hashCodes})",
                $@"DELETE FROM [{databaseName}].[dbo].[__MergableMigrationHistory]
WHERE HashCode IN ({hashCodes})"
            };
            return sql;
        }
    }
}