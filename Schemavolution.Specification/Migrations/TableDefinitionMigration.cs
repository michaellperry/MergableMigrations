﻿using Schemavolution.Specification.Implementation;
using System.Collections.Immutable;

namespace Schemavolution.Specification.Migrations
{
    abstract class TableDefinitionMigration : Migration
    {
        internal abstract CreateTableMigration CreateTableMigration { get; }
        internal abstract string GenerateDefinitionSql();

        public TableDefinitionMigration(ImmutableList<Migration> prerequisites)
            : base(prerequisites)
        {
        }

        internal override void AddToParent()
        {
            CreateTableMigration.AddDefinition(this);
        }
    }
}
