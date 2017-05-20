namespace MergableMigrations.Specification.Implementation
{
    class CreateDatabaseMigration : Migration
    {
        private readonly string _name;

        public string DatabaseName => _name;

        public CreateDatabaseMigration(string name)
        {
            _name = name;
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string[] sql =
            {
                $"CREATE DATABASE [{_name}]"
            };

            return sql;
        }

        public bool Equals(CreateDatabaseMigration other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (obj is CreateDatabaseMigration)
                return Equals((CreateDatabaseMigration)obj);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = nameof(CreateDatabaseMigration).Sha356Hash();
                if (_name != null)
                    hashCode = (hashCode * 53) ^ _name.Sha356Hash();
                return hashCode;
            }
        }
    }
}
