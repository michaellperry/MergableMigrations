using MergableMigrations.Specification.Implementation;
using MergableMigrations.Specification.Migrations;
using System.Collections.Generic;

namespace MergableMigrations.Specification
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
