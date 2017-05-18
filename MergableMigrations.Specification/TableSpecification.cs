using System;

namespace MergableMigrations.Specification
{
    public class TableSpecification
    {
        public ColumnSpecification CreateIntColumn(string name, bool nullable = false)
        {
            throw new NotImplementedException();
        }

        public ColumnSpecification CreateStringColumn(string name, int length, bool nullable = false)
        {
            throw new NotImplementedException();
        }

        public PrimaryKeySpecification CreatePrimaryKey(params ColumnSpecification[] columns)
        {
            throw new NotImplementedException();
        }

        public UniqueIndexSpecification CreateUniqueIndex(params ColumnSpecification[] columns)
        {
            throw new NotImplementedException();
        }

        public IndexSpecification CreateIndex(params ColumnSpecification[] columns)
        {
            throw new NotImplementedException();
        }
    }
}