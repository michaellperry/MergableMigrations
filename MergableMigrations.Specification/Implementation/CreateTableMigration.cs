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
    }
}