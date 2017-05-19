using System.Collections.Immutable;

namespace MergableMigrations.Specification.Implementation
{
    public class MigrationHistory
    {
        private readonly ImmutableList<Migration> _migrations;
        private readonly ImmutableHashSet<Migration> _migrationSet;

        public MigrationHistory()
        {
            _migrations = ImmutableList<Migration>.Empty;
            _migrationSet = ImmutableHashSet<Migration>.Empty;
        }

        private MigrationHistory(ImmutableList<Migration> migrations, ImmutableHashSet<Migration> migrationSet)
        {
            _migrations = migrations;
            _migrationSet = migrationSet;
        }

        public MigrationHistory Add(Migration migration)
        {
            return new MigrationHistory(
                _migrations.Add(migration),
                _migrationSet.Add(migration));
        }

        public MigrationDelta Subtract(MigrationHistory migrationHistory)
        {
            return new MigrationDelta(_migrations.RemoveAll(x =>
                migrationHistory.Contains(x)));
        }

        public bool Contains(Migration migration)
        {
            return _migrationSet.Contains(migration);
        }
    }
}
