using System.Collections.Generic;

namespace MergableMigrations.EF6.Loader
{
    class LookaheadEnumerator<T>
    {
        private IEnumerator<T> _enumerator;
        private bool _more;

        public LookaheadEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public T Current => _enumerator.Current;
        public bool More => _more;

        public bool MoveNext()
        {
            _more = _enumerator.MoveNext();
            return _more;
        }
    }
}