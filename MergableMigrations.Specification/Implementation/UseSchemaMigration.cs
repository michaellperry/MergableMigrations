using System;
using System.Collections.Generic;

namespace MergableMigrations.Specification.Implementation
{
    class UseSchemaMigration : Migration
    {
        private readonly CreateDatabaseMigration _parent;
        private readonly string _schemaName;

        public string DatabaseName => _parent.DatabaseName;
        public string SchemaName => _schemaName;

        public UseSchemaMigration(CreateDatabaseMigration parent, string schemaName)
        {
            _parent = parent;
            _schemaName = schemaName;
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string[] sql = { };
            return sql;
        }

        public bool Equals(UseSchemaMigration other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(_schemaName, other._schemaName) && Equals(_parent, other._parent);
        }

        public override bool Equals(object obj)
        {
            if (obj is UseSchemaMigration)
                return Equals((UseSchemaMigration)obj);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = nameof(UseSchemaMigration).Sha356Hash();
                if (_parent != null)
                    hashCode = (hashCode * 53) ^ _parent.GetHashCode();
                if (_schemaName != null)
                    hashCode = (hashCode * 53) ^ _schemaName.Sha356Hash();
                return hashCode;
            }
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(UseSchemaMigration),
                new Dictionary<string, string>
                {
                    [nameof(SchemaName)] = SchemaName
                },
                GetHashCode(),
                new List<int> { _parent.GetHashCode() });
        }
    }
}