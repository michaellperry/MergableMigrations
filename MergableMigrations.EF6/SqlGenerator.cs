using MergableMigrations.Specification;
using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.EF6
{
    public class SqlGenerator
    {
        private readonly IMigrations _migrations;
        private readonly MigrationHistory _migrationHistory;

        public SqlGenerator(IMigrations migrations, MigrationHistory migrationHistory)
        {
            _migrations = migrations;
            _migrationHistory = migrationHistory;
        }

        public string[] Generate()
        {
            var model = new ModelSpecification();
            _migrations.AddMigrations(model);
            MigrationHistory migrations = model.Migrations;
            MigrationHistory difference = migrations.Subtract(_migrationHistory);

            string[] sql =
            {
                ""
            };

            return sql;
        }
    }
}