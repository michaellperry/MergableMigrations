using System.Collections.Immutable;
using System.Linq;

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

        public bool Any => _migrations.Any();
        public Migration Head => _migrations.First();

        public MigrationHistory Add(Migration migration)
        {
            return new MigrationHistory(
                _migrations.Add(migration),
                _migrationSet.Add(migration));
        }

        public MigrationHistory Subtract(MigrationHistory migrationHistory)
        {
            return new MigrationHistory(
                _migrations.RemoveAll(x =>
                    migrationHistory._migrationSet.Contains(x)),
                _migrationSet.Except(migrationHistory._migrations));
        }
    }
}
