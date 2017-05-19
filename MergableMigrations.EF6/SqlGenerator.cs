using MergableMigrations.Specification;

namespace MergableMigrations.EF6
{
    public class SqlGenerator
    {
        private readonly IMigrations _migrations;

        public SqlGenerator(IMigrations migrations)
        {
            _migrations = migrations;
            Generate();
        }

        public string Generate()
        {
            var model = new ModelSpecification();
            _migrations.AddMigrations(model);

            return "";
        }
    }
}