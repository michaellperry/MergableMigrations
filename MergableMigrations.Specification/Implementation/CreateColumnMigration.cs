using System;
using MergableMigrations.Specification.Implementation;
using System.Collections.Generic;

namespace MergableMigrations.Specification
{
    class CreateColumnMigration : Migration
    {
        private readonly CreateTableMigration _parent;
        private readonly string _columnName;
        private readonly string _typeDescriptor;

        public string DatabaseName => _parent.DatabaseName;
        public string SchemaName => _parent.SchemaName;
        public string TableName => _parent.TableName;
        public string ColumnName => _columnName;
        public string TypeDescriptor => _typeDescriptor;

        public CreateColumnMigration(CreateTableMigration parent, string columnName, string typeDescriptor)
        {
            _parent = parent;
            _columnName = columnName;
            _typeDescriptor = typeDescriptor;
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string[] sql =
            {
                $"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]\r\n    ADD [{ColumnName}] {TypeDescriptor}"
            };

            return sql;
        }

        public bool Equals(CreateColumnMigration other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(_columnName, other._columnName) && Equals(_typeDescriptor, other._typeDescriptor) && Equals(_parent, other._parent);
        }

        public override bool Equals(object obj)
        {
            if (obj is CreateColumnMigration)
                return Equals((CreateColumnMigration)obj);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = nameof(CreateColumnMigration).Sha356Hash();
                if (_parent != null)
                    hashCode = (hashCode * 53) ^ _parent.GetHashCode();
                if (_columnName != null)
                    hashCode = (hashCode * 53) ^ _columnName.Sha356Hash();
                if (_typeDescriptor != null)
                    hashCode = (hashCode * 53) ^ _typeDescriptor.Sha356Hash();
                return hashCode;
            }
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(CreateColumnMigration),
                new Dictionary<string, string>
                {
                    [nameof(ColumnName)] = ColumnName,
                    [nameof(TypeDescriptor)] = TypeDescriptor
                },
                GetHashCode(),
                new List<int> { _parent.GetHashCode() });
        }
    }
}
