using System;

namespace MergableMigrations.Specification
{
    public class DatabaseSpecification
    {
        public SchemaSpecification UseSchema(string str)
        {
            return new SchemaSpecification();
        }
    }
}