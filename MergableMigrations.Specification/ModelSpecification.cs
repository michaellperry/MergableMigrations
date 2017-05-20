using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    public class ModelSpecification
    {
        private MigrationHistoryBuilder _migrationHistoryBuilder =
            new MigrationHistoryBuilder();

        public MigrationHistory MigrationHistory =>
            _migrationHistoryBuilder.MigrationHistory;

        public DatabaseSpecification CreateDatabase(string name)
        {
            var migration = new CreateDatabaseMigration(name);
            _migrationHistoryBuilder.Append(migration);
            return new DatabaseSpecification(migration, _migrationHistoryBuilder);
        }
    }
}