using Schemavolution.Specification.Implementation;
using Schemavolution.Specification.Migrations;
using System.Collections.Generic;

namespace Schemavolution.Specification
{
    public class CustomSqlSpecification : Specification
    {
        private CustomSqlMigration _migration;

        internal override IEnumerable<Migration> Migrations => new[] { _migration };

        internal CustomSqlSpecification(CustomSqlMigration migration, MigrationHistoryBuilder migrationHistoryBuilder) :
            base(migrationHistoryBuilder)
        {
            _migration = migration;
        }
    }
}
