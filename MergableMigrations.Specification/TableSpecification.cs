﻿using System;
using MergableMigrations.Specification.Implementation;
using System.Linq;

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
            return new ColumnSpecification(childMigration);
        }

        public ColumnSpecification CreateStringColumn(string columnName, int length, bool nullable = false)
        {
            var childMigration = new CreateColumnMigration(
                _migration,
                columnName, $"NVARCHAR({length}) {(nullable ? "NULL" : "NOT NULL")}");
            _migration.AddColumn(childMigration);
            _migrationHistoryBuilder.Append(childMigration);
            return new ColumnSpecification(childMigration);
        }

        public PrimaryKeySpecification CreatePrimaryKey(params ColumnSpecification[] columns)
        {
            var childMigration = new PrimaryKeyMigration(
                _migration,
                columns.Select(c => c.Migration));
            _migrationHistoryBuilder.Append(childMigration);
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