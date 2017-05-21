using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace MergableMigrations.Specification.Implementation
{
    class CreateTableMigration : Migration
    {
        private readonly UseSchemaMigration _parent;
        private readonly string _tableName;

        private ImmutableList<CreateColumnMigration> _columns =
            ImmutableList<CreateColumnMigration>.Empty;

        public string DatabaseName => _parent.DatabaseName;
        public string SchemaName => _parent.SchemaName;
        public string TableName => _tableName;

        public CreateTableMigration(UseSchemaMigration parent, string tableName)
        {
            _parent = parent;
            _tableName = tableName;
        }

        internal void AddColumn(CreateColumnMigration childMigration)
        {
            _columns = _columns.Add(childMigration);
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string createTable;
            string head = $"CREATE TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]";
            if (_columns.Any())
            {
                createTable = $"{head}({string.Join(",", _columns.Select(GenerateColumnSql))})";
            }
            else
            {
                createTable = head;
            }

            string[] sql =
            {
                createTable
            };
            migrationsAffected.AppendAll(_columns);

            return sql;
        }

        private string GenerateColumnSql(CreateColumnMigration column)
        {
            return $"\r\n    [{column.ColumnName}] {column.TypeDescriptor}";
        }

        public bool Equals(CreateTableMigration other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(_tableName, other._tableName) && Equals(_parent, other._parent);
        }

        public override bool Equals(object obj)
        {
            if (obj is CreateTableMigration)
                return Equals((CreateTableMigration)obj);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = nameof(CreateTableMigration).Sha356Hash();
                if (_parent != null)
                    hashCode = (hashCode * 53) ^ _parent.GetHashCode();
                if (_tableName != null)
                    hashCode = (hashCode * 53) ^ _tableName.Sha356Hash();
                return hashCode;
            }
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(CreateTableMigration),
                new Dictionary<string, string>
                {
                    [nameof(TableName)] = TableName
                },
                GetHashCode(),
                new List<int> { _parent.GetHashCode() });
        }
    }
}