using Schemavolution.Specification.Migrations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Schemavolution.Specification.Implementation
{
    abstract class IndexMigration : TableDefinitionMigration
    {
        protected IndexMigration(ImmutableList<Migration> prerequisites) :
            base(prerequisites)
        {
        }

        public abstract string DatabaseName { get; }
        public abstract string SchemaName { get; }
        public abstract string TableName { get; }
        public abstract IEnumerable<CreateColumnMigration> Columns { get; }
    }
}
