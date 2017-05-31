using MergableMigrations.Specification.Implementation;
using MergableMigrations.Specification.Migrations;
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
            return CreateColumn(columnName, "INT IDENTITY (1,1)", false, null);
        }

        public ColumnSpecification CreateBigIntColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "BIGINT", nullable, "0");
        }

        public ColumnSpecification CreateIntColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "INT", nullable, "0");
        }

        public ColumnSpecification CreateSmallIntColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "SMALLINT", nullable, "0");
        }

        public ColumnSpecification CreateTinyIntColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "TINYINT", nullable, "0");
        }

        public ColumnSpecification CreateBitColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "BIT", nullable, "0");
        }

        public ColumnSpecification CreateMoneyColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "MONEY", nullable, "0");
        }

        public ColumnSpecification CreateSmallMoneyColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "SMALLMONEY", nullable, "0");
        }

        public ColumnSpecification CreateDecimalColumn(string columnName, int precision = 18, int scale = 0, bool nullable = false)
        {
            return CreateColumn(columnName, $"DECIMAL({precision},{scale})", nullable, "0");
        }

        public ColumnSpecification CreateFloatColumn(string columnName, int mantissa = 57, bool nullable = false)
        {
            return CreateColumn(columnName, $"FLOAT({mantissa})", nullable, "0");
        }

        public ColumnSpecification CreateRealColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "REAL", nullable, "0");
        }

        public ColumnSpecification CreateDateColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "DATE", nullable, "GETUTCDATE()");
        }

        public ColumnSpecification CreateDateTimeColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "DATETIME", nullable, "GETUTCDATE()");
        }

        public ColumnSpecification CreateSmallDateTimeColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "SMALLDATETIME", nullable, "GETUTCDATE()");
        }

        public ColumnSpecification CreateDateTime2Column(string columnName, int fractionalSeconds = 7, bool nullable = false)
        {
            return CreateColumn(columnName, $"DATETIME2({fractionalSeconds})", nullable, "GETUTCDATE()");
        }

        public ColumnSpecification CreateTimeColumn(string columnName, int fractionalSeconds = 7, bool nullable = false)
        {
            return CreateColumn(columnName, $"TIME({fractionalSeconds})", nullable, "CONVERT (TIME, GETUTCDATE())");
        }

        public ColumnSpecification CreateDateTimeOffsetColumn(string columnName, int fractionalSeconds = 7, bool nullable = false)
        {
            return CreateColumn(columnName, $"DATETIMEOFFSET({fractionalSeconds})", nullable, "SYSDATETIMEOFFSET()");
        }

        public ColumnSpecification CreateStringColumn(string columnName, int length, bool nullable = false)
        {
            return CreateColumn(columnName, $"NVARCHAR({length})", nullable, "N''");
        }

        public ColumnSpecification CreateFixedStringColumn(string columnName, int length, bool nullable = false)
        {
            return CreateColumn(columnName, $"NCHAR({length})", nullable, "N''");
        }

        public ColumnSpecification CreateTextColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "NTEXT", nullable, "N''");
        }

        public ColumnSpecification CreateStringMaxColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "NVARCHAR(MAX)", nullable, "N''");
        }

        public ColumnSpecification CreateAsciiStringColumn(string columnName, int length, bool nullable = false)
        {
            return CreateColumn(columnName, $"VARCHAR({length})", nullable, "''");
        }

        public ColumnSpecification CreateFixedAsciiStringColumn(string columnName, int length, bool nullable = false)
        {
            return CreateColumn(columnName, $"CHAR({length})", nullable, "''");
        }

        public ColumnSpecification CreateAsciiTextColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "TEXT", nullable, "''");
        }

        public ColumnSpecification CreateAsciiStringMaxColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "VARCHAR(MAX)", nullable, "''");
        }

        public ColumnSpecification CreateGuidColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "UNIQUEIDENTIFIER", nullable, "'00000000-0000-0000-0000-000000000000'");
        }

        private ColumnSpecification CreateColumn(string columnName, string typeDescriptor, bool nullable, string defaultExpression)
        {
            var childMigration = new CreateColumnMigration(
                _migration,
                columnName, $"{typeDescriptor} {(nullable ? "NULL" : "NOT NULL")}",
                nullable ? null : defaultExpression);
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