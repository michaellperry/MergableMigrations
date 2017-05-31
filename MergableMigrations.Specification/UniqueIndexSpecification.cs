using MergableMigrations.Specification.Implementation;
using MergableMigrations.Specification.Migrations;

namespace MergableMigrations.Specification
{
    public class UniqueIndexSpecification
    {
        private CreateUniqueIndexMigration _migration;
        private MigrationHistoryBuilder _migrationHistoryBuilder;

        internal CreateUniqueIndexMigration Migration => _migration;

        internal UniqueIndexSpecification(CreateUniqueIndexMigration migration, MigrationHistoryBuilder migrationHistoryBuilder)
        {
            _migration = migration;
            _migrationHistoryBuilder = migrationHistoryBuilder;
        }

        public ForeignKeySpecification CreateForeignKey(PrimaryKeySpecification referencing, bool cascadeDelete = false, bool cascadeUpdate = false)
        {
            var childMigration = new CreateForeignKeyMigration(
                _migration,
                referencing.Migration,
                cascadeDelete,
                cascadeUpdate);
            _migrationHistoryBuilder.Append(childMigration);
            childMigration.AddToPrerequisites();
            return new ForeignKeySpecification();
        }
    }
}
