using MergableMigrations.Specification.Implementation;
using MergableMigrations.Specification.Migrations;

namespace MergableMigrations.Specification
{
    public class DatabaseSpecification
    {
        private readonly string _databaseName;
        private readonly MigrationHistoryBuilder _migrationHistoryBuilder = new MigrationHistoryBuilder();

        public MigrationHistory MigrationHistory => _migrationHistoryBuilder.MigrationHistory;

        public DatabaseSpecification(string databaseName)
        {
            _databaseName = databaseName;
        }

        public SchemaSpecification UseSchema(string schemaName)
        {
            var migration = new UseSchemaMigration(_databaseName, schemaName);
            _migrationHistoryBuilder.Append(migration);
            migration.AddToPrerequisites();
            return new SchemaSpecification(migration, _migrationHistoryBuilder);
        }
    }
}