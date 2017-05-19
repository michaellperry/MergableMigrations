using System;
using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    public class TableSpecification
    {
        private readonly string _databaseName;
        private readonly string _schemaName;
        private readonly string _tableName;
        private readonly MigrationHistoryBuilder _migrationHistoryBuilder;

        public TableSpecification(string databaseName, string schemaName, string tableName, MigrationHistoryBuilder migrationHistoryBuilder)
        {
            _databaseName = databaseName;
            _schemaName = schemaName;
            _tableName = tableName;
            _migrationHistoryBuilder = migrationHistoryBuilder;
        }

        public ColumnSpecification CreateIntColumn(string name, bool nullable = false)
        {
            return new ColumnSpecification();
        }

        public ColumnSpecification CreateStringColumn(string name, int length, bool nullable = false)
        {
            return new ColumnSpecification();
        }

        public PrimaryKeySpecification CreatePrimaryKey(params ColumnSpecification[] columns)
        {
            return new PrimaryKeySpecification();
        }

        public UniqueIndexSpecification CreateUniqueIndex(params ColumnSpecification[] columns)
        {
            throw new NotImplementedException();
        }

        public IndexSpecification CreateIndex(params ColumnSpecification[] columns)
        {
            return new IndexSpecification();
        }
    }
}