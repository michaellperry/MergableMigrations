using System;
using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    public class ModelSpecification
    {
        public MigrationHistory MigrationHistory { get; } = new MigrationHistory();

        public DatabaseSpecification CreateDatabase(string name)
        {
            MigrationHistory.Append(new CreateDatabaseMigration(name));
            return new DatabaseSpecification();
        }
    }
}