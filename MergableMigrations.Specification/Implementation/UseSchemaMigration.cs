using System.Collections.Generic;
using System.Numerics;

namespace MergableMigrations.Specification.Implementation
{
    class UseSchemaMigration : Migration
    {
        private readonly string _databaseName;
        private readonly string _schemaName;

        public string DatabaseName => _databaseName;
        public string SchemaName => _schemaName;

        public UseSchemaMigration(string databaseName, string schemaName)
        {
            _databaseName = databaseName;
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
                _schemaName.Sha256Hash());
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(UseSchemaMigration),
                new Dictionary<string, string>
                {
                    [nameof(DatabaseName)] = DatabaseName,
                    [nameof(SchemaName)] = SchemaName
                },
                Sha256Hash,
                new Dictionary<string, IEnumerable<BigInteger>>
                {
                });
        }
    }
}