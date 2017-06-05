using Schemavolution.Specification;

namespace Schemavolution.DDD
{
    public class AggregateRoot
    {
        public PrimaryKeySpecification PrimaryKey { get; }

        public AggregateRoot(PrimaryKeySpecification primaryKey)
        {
            PrimaryKey = primaryKey;
        }
    }
}
