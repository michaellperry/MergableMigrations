using System;
using System.Collections.Immutable;
using System.Linq;

namespace MergableMigrations.Specification.Implementation
{
    class CreateTableMigration : Migration
    {
        private readonly string _databaseName;
        private readonly string _schemaName;
        private readonly string _tableName;

        private ImmutableList<CreateColumnMigration> _columns =
            ImmutableList<CreateColumnMigration>.Empty;

        public CreateTableMigration(string databaseName, string schemaName, string tableName)
        {
            _databaseName = databaseName;
            _schemaName = schemaName;
            _tableName = tableName;
        }

        internal void AddColumn(CreateColumnMigration childMigration)
        {
            _columns = _columns.Add(childMigration);
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string createTable;
            string head = $"CREATE TABLE [{_databaseName}].[{_schemaName}].[{_tableName}]";
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
            return $"\r\n    [{column.Name}] {column.TypeDescriptor}";
        }

        public bool Equals(CreateTableMigration other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(_databaseName, other._databaseName) && Equals(_schemaName, other._schemaName) && Equals(_tableName, other._tableName);
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
                var hashCode = 47;
                if (_databaseName != null)
                    hashCode = (hashCode * 53) ^ _databaseName.GetHashCode();
                if (_schemaName != null)
                    hashCode = (hashCode * 53) ^ _schemaName.GetHashCode();
                if (_tableName != null)
                    hashCode = (hashCode * 53) ^ _tableName.GetHashCode();
                return hashCode;
            }
        }
    }
}