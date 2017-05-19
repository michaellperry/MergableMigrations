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
            _migrationHistoryBuilder.Append(new CreateDatabaseMigration(name));
            return new DatabaseSpecification(name, _migrationHistoryBuilder);
        }
    }
}