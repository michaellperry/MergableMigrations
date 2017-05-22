using MergableMigrations.Specification.Implementation;
using System.Collections.Generic;

namespace MergableMigrations.Specification
{
    public class ModelSpecification
    {
        private MigrationHistoryBuilder _migrationHistoryBuilder =
            new MigrationHistoryBuilder();

        public string DatabaseName { get; private set; }
        public MigrationHistory MigrationHistory =>
            _migrationHistoryBuilder.MigrationHistory;

        public DatabaseSpecification CreateDatabase(string databaseName)
        {
            DatabaseName = databaseName;
            var migration = new CreateDatabaseMigration(databaseName);
            _migrationHistoryBuilder.Append(migration);
            return new DatabaseSpecification(migration, _migrationHistoryBuilder);
        }
    }
}