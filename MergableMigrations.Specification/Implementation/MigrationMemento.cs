using System.Collections.Generic;
using System.Collections.Immutable;

namespace MergableMigrations.Specification.Implementation
{
    public class MigrationMemento
    {
        public string Type { get; }
        public ImmutableDictionary<string, string> Attributes { get; }
        public int HashCode { get; }
        public ImmutableList<int> Prerequisites { get; }

        public MigrationMemento(string type, IDictionary<string, string> attributes, int hashCode, IEnumerable<int> prerequisites)
        {
            Type = type;
            Attributes = attributes.ToImmutableDictionary();
            HashCode = hashCode;
            Prerequisites = prerequisites.ToImmutableList();
        }
    }
}