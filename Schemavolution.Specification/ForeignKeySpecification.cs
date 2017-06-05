using Schemavolution.Specification.Implementation;
using System.Collections.Generic;
using System.Linq;

namespace Schemavolution.Specification
{
    public class ForeignKeySpecification : Specification
    {
        internal override IEnumerable<Migration> Migrations => Enumerable.Empty<Migration>();

        internal ForeignKeySpecification(MigrationHistoryBuilder migrationHistoryBuilder) :
            base(migrationHistoryBuilder)
        {
        }
    }
}