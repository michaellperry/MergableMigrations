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
    }
}