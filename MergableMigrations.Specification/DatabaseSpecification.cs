using System;
using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    public class DatabaseSpecification
    {
        private readonly CreateDatabaseMigration _parent;
        private readonly MigrationHistoryBuilder _migrationHistoryBuilder;

        private string DatabaseName => _parent.DatabaseName;

        internal DatabaseSpecification(CreateDatabaseMigration parent, MigrationHistoryBuilder migrationHistoryBuilder)
        {
            _parent = parent;
            _migrationHistoryBuilder = migrationHistoryBuilder;
        }

        public SchemaSpecification UseSchema(string schemaName)
        {
            var migration = new UseSchemaMigration(_parent, schemaName);
            _migrationHistoryBuilder.Append(migration);
            return new SchemaSpecification(
                migration, _migrationHistoryBuilder);
        }
    }
}