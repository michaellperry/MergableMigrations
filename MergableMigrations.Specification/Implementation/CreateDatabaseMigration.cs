using System;
using System.Collections.Generic;

namespace MergableMigrations.Specification.Implementation
{
    class CreateDatabaseMigration : Migration
    {
        private readonly string _databaseName;

        public string DatabaseName => _databaseName;

        public CreateDatabaseMigration(string databaseName)
        {
            _databaseName = databaseName;
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string[] sql =
            {
                $"CREATE DATABASE [{_databaseName}]"
            };

            return sql;
        }

        public bool Equals(CreateDatabaseMigration other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(_databaseName, other._databaseName);
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
                if (_databaseName != null)
                    hashCode = (hashCode * 53) ^ _databaseName.Sha356Hash();
                return hashCode;
            }
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(CreateDatabaseMigration),
                new Dictionary<string, string>
                {
                    [nameof(DatabaseName)] = DatabaseName
                },
                GetHashCode(),
                new List<int> { });
        }
    }
}
