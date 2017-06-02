using MergableMigrations.EF6.Generator;
using MergableMigrations.Specification;
using MergableMigrations.Specification.Implementation;
using System;
using System.Linq;

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

        public string[] Generate(string databaseName)
        {
            var newMigrations = GetMigrationHistory(databaseName);
            var ahead = _migrationHistory.Subtract(newMigrations);
            if (ahead.Any)
                throw new InvalidOperationException(
                    "The target database is ahead of the desired migration. You can force a rollback, which may destroy data.");
            var difference = newMigrations.Subtract(_migrationHistory);

            var generator = new ForwardGenerator(databaseName, difference);

            while (generator.Any)
            {
                generator.AddMigration(generator.Head);
            }

            return generator.Sql.ToArray();
        }

        public string[] GenerateRollbackSql(string databaseName)
        {
            var newMigrations = GetMigrationHistory(databaseName);
            var ahead = _migrationHistory.Subtract(newMigrations);

            var generator = new RollbackGenerator(databaseName, ahead);

            while (generator.Any)
            {
                generator.AddMigration(generator.Head);
            }

            return generator.Sql.ToArray();
        }

        private MigrationHistory GetMigrationHistory(string databaseName)
        {
            var databaseSpecification = new DatabaseSpecification(databaseName);
            _migrations.AddMigrations(databaseSpecification);
            return databaseSpecification.MigrationHistory;
        }
    }
}