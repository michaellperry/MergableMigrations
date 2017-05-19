using System;

namespace MergableMigrations.Specification
{
    public class IndexSpecification
    {
        public ForeignKeySpecification CreateForeignKey(PrimaryKeySpecification referencing, bool cascadeDelete = false, bool cascadeUpdate = false)
        {
            return new ForeignKeySpecification();
        }
    }
}