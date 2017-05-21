using System;
using System.Collections.Generic;
using System.Numerics;

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

        protected override BigInteger ComputeSha256Hash()
        {
            return nameof(UseSchemaMigration).Sha256Hash().Concatenate(
                _parent.Sha256Hash,
                _schemaName.Sha256Hash());
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(UseSchemaMigration),
                new Dictionary<string, string>
                {
                    [nameof(SchemaName)] = SchemaName
                },
                Sha256Hash,
                new Dictionary<string, IEnumerable<BigInteger>>
                {
                    ["Parent"] = new BigInteger[] { _parent.Sha256Hash }
                });
        }
    }
}