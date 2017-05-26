using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    public class PrimaryKeySpecification
    {
        private readonly CreatePrimaryKeyMigration _migration;
        private readonly MigrationHistoryBuilder _migrationHistoryBuilder;

        internal CreatePrimaryKeyMigration Migration => _migration;

        internal PrimaryKeySpecification(CreatePrimaryKeyMigration migration, MigrationHistoryBuilder migrationHistoryBuilder)
        {
            _migration = migration;
            _migrationHistoryBuilder = migrationHistoryBuilder;
        }

        public ForeignKeySpecification CreateForeignKey(PrimaryKeySpecification referencing, bool cascadeDelete = false, bool cascadeUpdate = false)
        {
            var childMigration = new CreateForeignKeyMigration(
                _migration,
                referencing._migration,
                cascadeDelete,
                cascadeUpdate);
            _migrationHistoryBuilder.Append(childMigration);
            childMigration.AddToPrerequisites();
            return new ForeignKeySpecification();
        }
    }
}