using System;
using System.Collections.Generic;
using Schemavolution.Specification.Implementation;
using Schemavolution.Specification.Migrations;
using System.Linq;

namespace Schemavolution.Specification
{
    public class ColumnSpecification : Specification
    {
        private readonly CreateColumnMigration _migration;

        internal CreateColumnMigration Migration => _migration;
        internal override IEnumerable<Migration> Migrations => new[] { _migration };

        internal ColumnSpecification(CreateColumnMigration migration, MigrationHistoryBuilder migrationHistoryBuilder) :
            base(migrationHistoryBuilder)
        {
            _migration = migration;
        }
    }
}