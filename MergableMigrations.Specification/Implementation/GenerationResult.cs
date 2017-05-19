using System.Collections.Immutable;

namespace MergableMigrations.Specification.Implementation
{
    public class GenerationResult
    {
        public ImmutableList<string> Sql { get; }
        public MigrationHistory Migrations { get; }

        public GenerationResult(ImmutableList<string> sql, MigrationHistory migrations)
        {
            Sql = sql;
            Migrations = migrations;
        }
    }
}
