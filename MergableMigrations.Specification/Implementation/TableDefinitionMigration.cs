using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    abstract class TableDefinitionMigration : Migration
    {
        internal abstract string GenerateDefinitionSql();
    }
}
