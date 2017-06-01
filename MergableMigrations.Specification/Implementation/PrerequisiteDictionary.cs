using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;

namespace MergableMigrations.Specification.Implementation
{
    public class PrerequisiteDictionary : IEnumerable<KeyValuePair<string, ImmutableList<BigInteger>>>
    {
        private ImmutableDictionary<string, ImmutableList<BigInteger>> _dictionary;

        public PrerequisiteDictionary()
        {
            _dictionary = ImmutableDictionary<string, ImmutableList<BigInteger>>.Empty;
        }

        public PrerequisiteDictionary(IDictionary<string, IEnumerable<BigInteger>> prerequisites)
        {
            _dictionary = prerequisites.ToImmutableDictionary(
                x => x.Key,
                x => x.Value.ToImmutableList());
        }

        public ImmutableList<BigInteger> this[string key]
        {
            get
            {
                if (_dictionary.TryGetValue(key, out var list))
                    return list;
                else
                    return ImmutableList<BigInteger>.Empty;
            }
        }

        public IEnumerator<KeyValuePair<string, ImmutableList<BigInteger>>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, ImmutableList<BigInteger>>>)_dictionary).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, ImmutableList<BigInteger>>>)_dictionary).GetEnumerator();
        }
    }
}
