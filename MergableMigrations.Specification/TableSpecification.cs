using System;

namespace MergableMigrations.Specification
{
    public class TableSpecification
    {
        public ColumnSpecification CreateIntColumn(string name, bool nullable = false)
        {
            return new ColumnSpecification();
        }

        public ColumnSpecification CreateStringColumn(string name, int length, bool nullable = false)
        {
            return new ColumnSpecification();
        }

        public PrimaryKeySpecification CreatePrimaryKey(params ColumnSpecification[] columns)
        {
            return new PrimaryKeySpecification();
        }

        public UniqueIndexSpecification CreateUniqueIndex(params ColumnSpecification[] columns)
        {
            throw new NotImplementedException();
        }

        public IndexSpecification CreateIndex(params ColumnSpecification[] columns)
        {
            return new IndexSpecification();
        }
    }
}