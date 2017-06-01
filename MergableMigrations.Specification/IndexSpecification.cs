using MergableMigrations.Specification.Implementation;
using MergableMigrations.Specification.Migrations;
using System.Collections.Generic;

namespace MergableMigrations.Specification
{
    public class IndexSpecification : Specification
    {
        private CreateIndexMigration _migration;

        internal CreateIndexMigration Migration => _migration;
        internal override IEnumerable<Migration> Migrations => new[] { _migration };

        internal IndexSpecification(CreateIndexMigration migration, MigrationHistoryBuilder migrationHistoryBuilder) :
            base(migrationHistoryBuilder)
        {
            _migration = migration;
        }

        public ForeignKeySpecification CreateForeignKey(PrimaryKeySpecification referencing, bool cascadeDelete = false, bool cascadeUpdate = false)
        {
            var childMigration = new CreateForeignKeyMigration(
                _migration,
                referencing.Migration,
                cascadeDelete,
                cascadeUpdate);
            MigrationHistoryBuilder.Append(childMigration);
            childMigration.AddToParent();
            return new ForeignKeySpecification(MigrationHistoryBuilder);
        }
    }
}