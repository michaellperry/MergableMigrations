using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification.Migrations
{
    abstract class TableDefinitionMigration : Migration
    {
        internal abstract CreateTableMigration CreateTableMigration { get; }
        internal abstract string GenerateDefinitionSql();

        internal override void AddToParent()
        {
            CreateTableMigration.AddDefinition(this);
        }
    }
}
