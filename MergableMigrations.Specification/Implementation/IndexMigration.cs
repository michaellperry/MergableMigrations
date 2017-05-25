using System.Collections.Generic;

namespace MergableMigrations.Specification.Implementation
{
    abstract class IndexMigration : TableDefinitionMigration
    {
        public abstract string DatabaseName { get; }
        public abstract string SchemaName { get; }
        public abstract string TableName { get; }
        public abstract IEnumerable<CreateColumnMigration> Columns { get; }
    }
}
