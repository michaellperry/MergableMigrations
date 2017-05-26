using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    abstract class TableDefinitionMigration : Migration
    {
        internal abstract CreateTableMigration CreateTableMigration { get; }
        internal abstract string GenerateDefinitionSql();

        internal override void AddToPrerequisites()
        {
            CreateTableMigration.AddDefinition(this);
        }
    }
}
