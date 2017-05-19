using System;
using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    public class SchemaSpecification
    {
        private readonly string _databaseName;
        private readonly string _schemaName;
        private readonly MigrationHistoryBuilder _migrationHistoryBuilder;

        public SchemaSpecification(string databaseName, string schemaName, MigrationHistoryBuilder migrationHistoryBuilder)
        {
            _databaseName = databaseName;
            _schemaName = schemaName;
            _migrationHistoryBuilder = migrationHistoryBuilder;
        }

        public TableSpecification CreateTable(string tableName)
        {
            _migrationHistoryBuilder.Append(new CreateTableMigration(_databaseName, _schemaName, tableName));
            return new TableSpecification(_databaseName, _schemaName, tableName, _migrationHistoryBuilder);
        }
    }
}