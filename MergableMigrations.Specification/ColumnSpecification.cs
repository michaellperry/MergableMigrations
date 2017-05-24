namespace MergableMigrations.Specification
{
    public class ColumnSpecification
    {
        private readonly CreateColumnMigration _migration;

        internal CreateColumnMigration Migration => _migration;

        internal ColumnSpecification(CreateColumnMigration migration)
        {
            _migration = migration;
        }
    }
}