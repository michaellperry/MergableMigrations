using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;

namespace MergableMigrations.Specification.Implementation
{
    public class MigrationMemento
    {
        public string Type { get; }
        public ImmutableDictionary<string, string> Attributes { get; }
        public BigInteger HashCode { get; }
        public ImmutableList<BigInteger> Prerequisites { get; }

        public MigrationMemento(string type, IDictionary<string, string> attributes, BigInteger hashCode, IEnumerable<BigInteger> prerequisites)
        {
            Type = type;
            Attributes = attributes.ToImmutableDictionary();
            HashCode = hashCode;
            Prerequisites = prerequisites.ToImmutableList();
        }
    }
}