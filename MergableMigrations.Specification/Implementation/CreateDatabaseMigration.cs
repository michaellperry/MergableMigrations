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

        public override GenerationResult GenerateSql()
        {
            string[] sql =
            {
                $"CREATE DATABASE [{_name}]"
            };

            var migrations = new MigrationHistory();
            migrations.Append(this);
            return new GenerationResult(sql.ToImmutableList(), migrations);
        }
    }
}
