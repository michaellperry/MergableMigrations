using System;
using System.Collections.Immutable;

namespace Schemavolution.Specification.Implementation
{
    public interface IGraphVisitor
    {
        ImmutableList<Migration> PullPrerequisitesForward(Migration migration, Migration origin, Func<Migration, bool> canOptimize);
    }
}
