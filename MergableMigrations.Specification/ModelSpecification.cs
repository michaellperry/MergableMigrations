using System;

namespace MergableMigrations.Specification
{
    public class ModelSpecification
    {
        public DatabaseSpecification CreateDatabase(string name)
        {
            return new DatabaseSpecification();
        }
    }
}