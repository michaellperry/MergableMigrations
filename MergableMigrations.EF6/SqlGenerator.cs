using MergableMigrations.Specification;
using MergableMigrations.Specification.Implementation;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System;
using System.Collections.Generic;

namespace MergableMigrations.EF6
{
    public class SqlGenerator
    {
        private readonly IMigrations _migrations;
        private readonly MigrationHistory _migrationHistory;

        public SqlGenerator(IMigrations migrations, MigrationHistory migrationHistory)
        {
            _migrations = migrations;
            _migrationHistory = migrationHistory;
        }

        public string[] Generate()
        {
            var model = new ModelSpecification();
            _migrations.AddMigrations(model);
            var newMigrations = model.MigrationHistory;
            var difference = newMigrations.Subtract(_migrationHistory);

            var sql = ImmutableList<string>.Empty;

            while (difference.Any)
            {
                var migrationsAffected = new MigrationHistoryBuilder();
                migrationsAffected.Append(difference.Head);
                string[] result = difference.Head.GenerateSql(migrationsAffected);
                sql = sql.AddRange(result);
                sql = sql.Add(GenerateInsertStatement(model.DatabaseName, migrationsAffected.MigrationHistory.GetMementos()));
                difference = difference.Subtract(migrationsAffected.MigrationHistory);
            }

            return sql.ToArray();
        }

        private string GenerateInsertStatement(string databaseName, IEnumerable<MigrationMemento> migrations)
        {
            string[] values = migrations.Select(migration => GenerateValue(migration)).ToArray();
            var insert = $@"INSERT INTO [{databaseName}].[dbo].[__MergableMigrationHistory]
                ([Type], [HashCode], [Body])
                VALUES{String.Join(",", values)}";
            return insert;
        }

        private string GenerateValue(MigrationMemento migration)
        {
            string body = JsonConvert.SerializeObject(new MigrationBody
            {
                Attributes = migration.Attributes.ToDictionary(
                    x => x.Key, x => x.Value),
                Prerequisites = migration.Prerequisites.ToDictionary(
                    x => x.Key, x => x.Value.Select(ToHexString).ToList())
            }).Replace("'", "''");
            string hex = ToHexString(migration.HashCode);
            return $"('{migration.Type}', {hex}, '{body}')";
        }

        private static string ToHexString(BigInteger hashCode)
        {
            return $"0x{hashCode.ToString("X")}";
        }
    }
}