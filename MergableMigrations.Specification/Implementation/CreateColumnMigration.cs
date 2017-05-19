using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification
{
    class CreateColumnMigration : Migration
    {
        private readonly string _databaseName;
        private readonly string _schemaName;
        private readonly string _tableName;
        private readonly string _columnName;
        private readonly string _typeDescriptor;

        public string Name => _columnName;
        public string TypeDescriptor => _typeDescriptor;

        public CreateColumnMigration(string databaseName, string schemaName, string tableName, string columnName, string typeDescriptor, MigrationHistoryBuilder migrationHistoryBuilder)
        {
            _databaseName = databaseName;
            _schemaName = schemaName;
            _tableName = tableName;
            _columnName = columnName;
            _typeDescriptor = typeDescriptor;
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string[] sql =
            {
                $"ALTER TABLE [{_databaseName}].[{_schemaName}].[{_tableName}]\n    ADD [{_columnName}] [{_typeDescriptor}]"
            };

            return sql;
        }

        public bool Equals(CreateColumnMigration other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(_databaseName, other._databaseName) && Equals(_schemaName, other._schemaName) && Equals(_tableName, other._tableName) && Equals(_columnName, other._columnName) && Equals(_typeDescriptor, other._typeDescriptor);
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
                var hashCode = 47;
                if (_databaseName != null)
                    hashCode = (hashCode * 53) ^ _databaseName.GetHashCode();
                if (_schemaName != null)
                    hashCode = (hashCode * 53) ^ _schemaName.GetHashCode();
                if (_tableName != null)
                    hashCode = (hashCode * 53) ^ _tableName.GetHashCode();
                if (_columnName != null)
                    hashCode = (hashCode * 53) ^ _columnName.GetHashCode();
                if (_typeDescriptor != null)
                    hashCode = (hashCode * 53) ^ _typeDescriptor.GetHashCode();
                return hashCode;
            }
        }
    }
}
