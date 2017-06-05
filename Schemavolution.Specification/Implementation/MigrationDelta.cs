using System;
using System.Collections.Immutable;
using System.Linq;

namespace Schemavolution.Specification.Implementation
{
    public class MigrationDelta
    {
        private readonly ImmutableList<Migration> _migrations;

        internal MigrationDelta(ImmutableList<Migration> migrations)
        {
            _migrations = migrations;
        }

        public bool Any => _migrations.Any();
        public Migration Head => _migrations.First();

        public bool Contains(Migration migration)
        {
            return _migrations.Contains(migration);
        }

        public MigrationDelta Subtract(MigrationHistory migrationHistory)
        {
            return new MigrationDelta(_migrations.RemoveAll(x =>
                migrationHistory.Contains(x)));
        }
    }
}
