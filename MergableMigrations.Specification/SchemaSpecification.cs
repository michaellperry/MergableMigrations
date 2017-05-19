using System;

namespace MergableMigrations.Specification
{
    public class SchemaSpecification
    {
        public TableSpecification CreateTable(string name)
        {
            return new TableSpecification();
        }
    }
}