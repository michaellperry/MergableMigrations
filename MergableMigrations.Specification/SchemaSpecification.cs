using MergableMigrations.Specification.Implementation;
using System;

namespace MergableMigrations.Specification
{
    public class SchemaSpecification
    {
        private readonly UseSchemaMigration _migration;
        private readonly MigrationHistoryBuilder _migrationHistoryBuilder;

        internal SchemaSpecification(UseSchemaMigration migration, MigrationHistoryBuilder migrationHistoryBuilder)
        {
            _migration = migration;
            _migrationHistoryBuilder = migrationHistoryBuilder;
        }

        public TableSpecification CreateTable(string tableName)
        {
            var migration = new CreateTableMigration(_migration, tableName);
            _migrationHistoryBuilder.Append(migration);
            return new TableSpecification(migration, _migrationHistoryBuilder);
        }
    }
}