using System;
using System.Collections.Immutable;

namespace MergableMigrations.Specification.Implementation
{
    public interface IGraphVisitor
    {
        ImmutableList<Migration> PullPrerequisitesForward(Migration migration, Migration origin, Func<Migration, bool> canOptimize);
    }
}
