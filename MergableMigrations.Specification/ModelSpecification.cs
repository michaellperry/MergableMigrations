using System;
using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    public class ModelSpecification
    {
        public MigrationHistory Migrations { get; } = MigrationHistory.Empty;

        public DatabaseSpecification CreateDatabase(string name)
        {
            return new DatabaseSpecification();
        }
    }
}