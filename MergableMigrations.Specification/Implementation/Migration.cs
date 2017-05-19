using System;

namespace MergableMigrations.Specification.Implementation
{
    public abstract class Migration
    {
        public abstract GenerationResult GenerateSql();
    }
}
