using System.Collections.Immutable;
using System.Linq;

namespace MergableMigrations.Specification.Implementation
{
    public class MigrationHistory
    {
        private readonly ImmutableList<Migration> _migrations;

        public MigrationHistory()
        {
            _migrations = ImmutableList<Migration>.Empty;
        }

        private MigrationHistory(ImmutableList<Migration> migrations)
        {
            _migrations = migrations;
        }

        public bool Any => _migrations.Any();
        public Migration Head => _migrations.First();

        public MigrationHistory Add(Migration migration)
        {
            return new MigrationHistory(_migrations.Add(migration));
        }

        public MigrationHistory Subtract(MigrationHistory migrationHistory)
        {
            return new MigrationHistory(_migrations.RemoveAll(x =>
                migrationHistory._migrations.Contains(x)));
        }
    }
}
