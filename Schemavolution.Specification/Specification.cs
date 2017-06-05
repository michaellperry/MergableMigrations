using Schemavolution.Specification.Implementation;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Schemavolution.Specification
{
    public abstract class Specification
    {
        private readonly MigrationHistoryBuilder _migrationHistoryBuilder;
        private readonly ImmutableList<Migration> _prerequisites;

        protected MigrationHistoryBuilder MigrationHistoryBuilder => _migrationHistoryBuilder;
        protected ImmutableList<Migration> Prerequisites => _prerequisites;
        internal abstract IEnumerable<Migration> Migrations { get; }

        protected Specification(MigrationHistoryBuilder migrationHistoryBuilder)
        {
            _migrationHistoryBuilder = migrationHistoryBuilder;
            _prerequisites = ImmutableList<Migration>.Empty;
        }

        protected Specification(MigrationHistoryBuilder migrationHistoryBuilder, ImmutableList<Migration> prerequisites)
        {
            _migrationHistoryBuilder = migrationHistoryBuilder;
            _prerequisites = prerequisites;
        }
    }
}
