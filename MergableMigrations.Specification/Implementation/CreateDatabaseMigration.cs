using System;
using System.Collections.Immutable;

namespace MergableMigrations.Specification.Implementation
{
    public class CreateDatabaseMigration : Migration
    {
        private readonly string _name;

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
    }
}
