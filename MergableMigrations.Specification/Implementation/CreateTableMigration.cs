namespace MergableMigrations.Specification.Implementation
{
    class CreateTableMigration : Migration
    {
        private readonly string _databaseName;
        private readonly string _schemaName;
        private readonly string _tableName;

        public CreateTableMigration(string databaseName, string schemaName, string tableName)
        {
            _databaseName = databaseName;
            _schemaName = schemaName;
            _tableName = tableName;
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string[] sql =
            {
                $"CREATE TABLE [{_databaseName}].[{_schemaName}].[{_tableName}]"
            };

            return sql;
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