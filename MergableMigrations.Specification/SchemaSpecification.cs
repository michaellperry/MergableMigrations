using System;
using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    public class SchemaSpecification
    {
        private readonly string _databaseName;
        private readonly string _schemaName;
        private readonly MigrationHistoryBuilder _migrationHistoryBuilder;

        internal SchemaSpecification(string databaseName, string schemaName, MigrationHistoryBuilder migrationHistoryBuilder)
        {
            _databaseName = databaseName;
            _schemaName = schemaName;
            _migrationHistoryBuilder = migrationHistoryBuilder;
        }

        public TableSpecification CreateTable(string tableName)
        {
            var migration = new CreateTableMigration(_databaseName, _schemaName, tableName);
            _migrationHistoryBuilder.Append(migration);
            return new TableSpecification(_databaseName, _schemaName, tableName, _migrationHistoryBuilder, migration);
        }
    }
}