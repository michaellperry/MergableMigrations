using MergableMigrations.Specification.Implementation;
using System.Collections.Generic;
using System.Linq;

namespace MergableMigrations.Specification
{
    public class CustomSqlSpecification : Specification
    {
        internal override IEnumerable<Migration> Migrations => Enumerable.Empty<Migration>();

        internal CustomSqlSpecification(MigrationHistoryBuilder migrationHistoryBuilder) :
            base(migrationHistoryBuilder)
        {

        }
    }
}
