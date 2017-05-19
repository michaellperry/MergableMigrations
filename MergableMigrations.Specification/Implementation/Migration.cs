using System;

namespace MergableMigrations.Specification.Implementation
{
    public abstract class Migration
    {
        public abstract string[] GenerateSql(MigrationHistoryBuilder migrationsAffected);
    }
}
