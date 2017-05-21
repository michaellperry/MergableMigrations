using System;
using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    public class TableSpecification
    {
        private readonly MigrationHistoryBuilder _migrationHistoryBuilder;
        private readonly CreateTableMigration _migration;

        internal TableSpecification(CreateTableMigration migration, MigrationHistoryBuilder migrationHistoryBuilder)
        {
            _migration = migration;
            _migrationHistoryBuilder = migrationHistoryBuilder;
        }

        public ColumnSpecification CreateIntColumn(string columnName, bool nullable = false)
        {
            var childMigration = new CreateColumnMigration(
                _migration,
                columnName, $"INT {(nullable ? "NULL" : "NOT NULL")}");
            _migration.AddColumn(childMigration);
            _migrationHistoryBuilder.Append(childMigration);
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