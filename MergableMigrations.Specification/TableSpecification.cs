using MergableMigrations.Specification.Implementation;
using MergableMigrations.Specification.Migrations;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MergableMigrations.Specification
{
    public class TableSpecification : Specification
    {
        private readonly CreateTableMigration _migration;

        internal override IEnumerable<Migration> Migrations => new[] { _migration };

        internal TableSpecification(CreateTableMigration migration, MigrationHistoryBuilder migrationHistoryBuilder) :
            base(migrationHistoryBuilder)
        {
            _migration = migration;
        }

        private TableSpecification(CreateTableMigration migration, MigrationHistoryBuilder migrationHistoryBuilder, ImmutableList<Migration> prerequisites) :
            base(migrationHistoryBuilder, prerequisites)
        {
            _migration = migration;
        }

        public TableSpecification After(params Specification[] specifications)
        {
            return new TableSpecification(_migration, MigrationHistoryBuilder,
                Prerequisites.AddRange(specifications.SelectMany(x => x.Migrations)));
        }

        public ColumnSpecification CreateIdentityColumn(string columnName)
        {
            return CreateColumn(columnName, "INT IDENTITY (1,1)", false);
        }

        public ColumnSpecification CreateBigIntColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "BIGINT", nullable);
        }

        public ColumnSpecification CreateIntColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "INT", nullable);
        }

        public ColumnSpecification CreateSmallIntColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "SMALLINT", nullable);
        }

        public ColumnSpecification CreateTinyIntColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "TINYINT", nullable);
        }

        public ColumnSpecification CreateBitColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "BIT", nullable);
        }

        public ColumnSpecification CreateMoneyColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "MONEY", nullable);
        }

        public ColumnSpecification CreateSmallMoneyColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "SMALLMONEY", nullable);
        }

        public ColumnSpecification CreateDecimalColumn(string columnName, int precision = 18, int scale = 0, bool nullable = false)
        {
            return CreateColumn(columnName, $"DECIMAL({precision},{scale})", nullable);
        }

        public ColumnSpecification CreateFloatColumn(string columnName, int mantissa = 57, bool nullable = false)
        {
            return CreateColumn(columnName, $"FLOAT({mantissa})", nullable);
        }

        public ColumnSpecification CreateRealColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "REAL", nullable);
        }

        public ColumnSpecification CreateDateColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "DATE", nullable);
        }

        public ColumnSpecification CreateDateTimeColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "DATETIME", nullable);
        }

        public ColumnSpecification CreateSmallDateTimeColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "SMALLDATETIME", nullable);
        }

        public ColumnSpecification CreateDateTime2Column(string columnName, int fractionalSeconds = 7, bool nullable = false)
        {
            return CreateColumn(columnName, $"DATETIME2({fractionalSeconds})", nullable);
        }

        public ColumnSpecification CreateTimeColumn(string columnName, int fractionalSeconds = 7, bool nullable = false)
        {
            return CreateColumn(columnName, $"TIME({fractionalSeconds})", nullable);
        }

        public ColumnSpecification CreateDateTimeOffsetColumn(string columnName, int fractionalSeconds = 7, bool nullable = false)
        {
            return CreateColumn(columnName, $"DATETIMEOFFSET({fractionalSeconds})", nullable);
        }

        public ColumnSpecification CreateStringColumn(string columnName, int length, bool nullable = false)
        {
            return CreateColumn(columnName, $"NVARCHAR({length})", nullable);
        }

        public ColumnSpecification CreateFixedStringColumn(string columnName, int length, bool nullable = false)
        {
            return CreateColumn(columnName, $"NCHAR({length})", nullable);
        }

        public ColumnSpecification CreateTextColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "NTEXT", nullable);
        }

        public ColumnSpecification CreateStringMaxColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "NVARCHAR(MAX)", nullable);
        }

        public ColumnSpecification CreateAsciiStringColumn(string columnName, int length, bool nullable = false)
        {
            return CreateColumn(columnName, $"VARCHAR({length})", nullable);
        }

        public ColumnSpecification CreateFixedAsciiStringColumn(string columnName, int length, bool nullable = false)
        {
            return CreateColumn(columnName, $"CHAR({length})", nullable);
        }

        public ColumnSpecification CreateAsciiTextColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "TEXT", nullable);
        }

        public ColumnSpecification CreateAsciiStringMaxColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "VARCHAR(MAX)", nullable);
        }

        public ColumnSpecification CreateGuidColumn(string columnName, bool nullable = false)
        {
            return CreateColumn(columnName, "UNIQUEIDENTIFIER", nullable);
        }

        private ColumnSpecification CreateColumn(string columnName, string typeDescriptor, bool nullable)
        {
            var childMigration = new CreateColumnMigration(
                _migration,
                columnName,
                typeDescriptor,
                nullable,
                Prerequisites);
            MigrationHistoryBuilder.Append(childMigration);
            childMigration.AddToParent();
            return new ColumnSpecification(childMigration, MigrationHistoryBuilder);
        }

        public PrimaryKeySpecification CreatePrimaryKey(params ColumnSpecification[] columns)
        {
            var childMigration = new CreatePrimaryKeyMigration(
                _migration,
                columns.Select(c => c.Migration),
                Prerequisites);
            MigrationHistoryBuilder.Append(childMigration);
            childMigration.AddToParent();
            return new PrimaryKeySpecification(childMigration, MigrationHistoryBuilder);
        }

        public UniqueIndexSpecification CreateUniqueIndex(params ColumnSpecification[] columns)
        {
            var childMigration = new CreateUniqueIndexMigration(
                _migration,
                columns.Select(c => c.Migration),
                Prerequisites);
            MigrationHistoryBuilder.Append(childMigration);
            childMigration.AddToParent();
            return new UniqueIndexSpecification(childMigration, MigrationHistoryBuilder);
        }

        public IndexSpecification CreateIndex(params ColumnSpecification[] columns)
        {
            var childMigration = new CreateIndexMigration(
                _migration,
                columns.Select(c => c.Migration),
                Prerequisites);
            MigrationHistoryBuilder.Append(childMigration);
            childMigration.AddToParent();
            return new IndexSpecification(childMigration, MigrationHistoryBuilder);
        }
    }
}