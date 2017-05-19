using System.Collections.Generic;

namespace MergableMigrations.Specification.Implementation
{
    public class MigrationHistoryBuilder
    {
        public MigrationHistory MigrationHistory { get; private set; } =
            new MigrationHistory();

        public void Append(Migration migration)
        {
            MigrationHistory = MigrationHistory.Add(migration);
        }

        public void AppendAll(IEnumerable<Migration> migrations)
        {
            MigrationHistory = MigrationHistory.AddAll(migrations);
        }
    }
}
