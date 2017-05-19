using System;
using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    public class DatabaseSpecification
    {
        private readonly string _databaseName;
        private readonly MigrationHistoryBuilder _migrationHistoryBuilder;

        public DatabaseSpecification(string databaseName, MigrationHistoryBuilder migrationHistoryBuilder)
        {
            _databaseName = databaseName;
            _migrationHistoryBuilder = migrationHistoryBuilder;
        }

        public SchemaSpecification UseSchema(string schemaName)
        {
            return new SchemaSpecification(_databaseName, schemaName, _migrationHistoryBuilder);
        }
    }
}