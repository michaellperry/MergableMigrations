using System;

namespace MergableMigrations.Specification.Implementation
{
    public class MigrationHistory
    {
        private MigrationHistory()
        {
        }

        public static MigrationHistory Empty { get; } = new MigrationHistory();

        public MigrationHistory Subtract(MigrationHistory migrationHistory)
        {
            return migrationHistory;
        }
    }
}
