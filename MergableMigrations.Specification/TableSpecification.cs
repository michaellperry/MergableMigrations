using MergableMigrations.Specification.Implementation;
using System.Linq;
using System;

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

        public ColumnSpecification CreateIdentityColumn(string columnName)
        {
            var childMigration = new CreateColumnMigration(
                _migration,
                columnName, "INT IDENTITY (1,1) NOT NULL");
            _migrationHistoryBuilder.Append(childMigration);
            childMigration.AddToPrerequisites();
            return new ColumnSpecification(childMigration);
        }

        public ColumnSpecification CreateIntColumn(string columnName, bool nullable = false)
        {
            var childMigration = new CreateColumnMigration(
                _migration,
                columnName, $"INT {(nullable ? "NULL" : "NOT NULL")}");
            _migrationHistoryBuilder.Append(childMigration);
            childMigration.AddToPrerequisites();
            return new ColumnSpecification(childMigration);
        }

        public ColumnSpecification CreateStringColumn(string columnName, int length, bool nullable = false)
        {
            var childMigration = new CreateColumnMigration(
                _migration,
                columnName, $"NVARCHAR({length}) {(nullable ? "NULL" : "NOT NULL")}");
            _migrationHistoryBuilder.Append(childMigration);
            childMigration.AddToPrerequisites();
            return new ColumnSpecification(childMigration);
        }

        public ColumnSpecification CreateGuidColumn(string columnName, bool nullable = false)
        {
            var childMigration = new CreateColumnMigration(
                _migration,
                columnName, $"UNIQUEIDENTIFIER {(nullable ? "NULL" : "NOT NULL")}");
            _migrationHistoryBuilder.Append(childMigration);
            childMigration.AddToPrerequisites();
            return new ColumnSpecification(childMigration);
        }

        public PrimaryKeySpecification CreatePrimaryKey(params ColumnSpecification[] columns)
        {
            var childMigration = new CreatePrimaryKeyMigration(
                _migration,
                columns.Select(c => c.Migration));
            _migrationHistoryBuilder.Append(childMigration);
            childMigration.AddToPrerequisites();
            return new PrimaryKeySpecification(childMigration, _migrationHistoryBuilder);
        }

        public UniqueIndexSpecification CreateUniqueIndex(params ColumnSpecification[] columns)
        {
            var childMigration = new CreateUniqueIndexMigration(
                _migration,
                columns.Select(c => c.Migration));
            _migrationHistoryBuilder.Append(childMigration);
            childMigration.AddToPrerequisites();
            return new UniqueIndexSpecification(childMigration, _migrationHistoryBuilder);
        }

        public IndexSpecification CreateIndex(params ColumnSpecification[] columns)
        {
            var childMigration = new CreateIndexMigration(
                _migration,
                columns.Select(c => c.Migration));
            _migrationHistoryBuilder.Append(childMigration);
            childMigration.AddToPrerequisites();
            return new IndexSpecification(childMigration, _migrationHistoryBuilder);
        }
    }
}