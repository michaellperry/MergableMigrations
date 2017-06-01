using MergableMigrations.Specification.Implementation;
using MergableMigrations.Specification.Migrations;
using System;
using System.Collections.Generic;

namespace MergableMigrations.Specification
{
    public class SchemaSpecification : Specification
    {
        private readonly UseSchemaMigration _migration;

        internal override IEnumerable<Migration> Migrations => new[] { _migration };

        internal SchemaSpecification(UseSchemaMigration migration, MigrationHistoryBuilder migrationHistoryBuilder) :
            base(migrationHistoryBuilder)
        {
            _migration = migration;
        }

        public TableSpecification CreateTable(string tableName)
        {
            var migration = new CreateTableMigration(_migration, tableName, Prerequisites);
            MigrationHistoryBuilder.Append(migration);
            migration.AddToParent();
            return new TableSpecification(migration, MigrationHistoryBuilder);
        }
    }
}