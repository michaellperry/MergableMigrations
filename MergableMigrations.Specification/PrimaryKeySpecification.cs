using MergableMigrations.Specification.Implementation;
using MergableMigrations.Specification.Migrations;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MergableMigrations.Specification
{
    public class PrimaryKeySpecification : Specification
    {
        private readonly CreatePrimaryKeyMigration _migration;

        internal CreatePrimaryKeyMigration Migration => _migration;
        internal override IEnumerable<Migration> Migrations => new[] { _migration };

        internal PrimaryKeySpecification(CreatePrimaryKeyMigration migration, MigrationHistoryBuilder migrationHistoryBuilder) :
            base(migrationHistoryBuilder)
        {
            _migration = migration;
        }

        public ForeignKeySpecification CreateForeignKey(PrimaryKeySpecification referencing, bool cascadeDelete = false, bool cascadeUpdate = false)
        {
            var childMigration = new CreateForeignKeyMigration(
                _migration,
                referencing._migration,
                cascadeDelete,
                cascadeUpdate,
                Prerequisites);
            MigrationHistoryBuilder.Append(childMigration);
            childMigration.AddToParent();
            return new ForeignKeySpecification(MigrationHistoryBuilder);
        }
    }
}