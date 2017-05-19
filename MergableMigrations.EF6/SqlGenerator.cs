using MergableMigrations.Specification;
using MergableMigrations.Specification.Implementation;
using System.Collections.Immutable;
using System.Linq;

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
            var newMigrations = model.MigrationHistory;
            var difference = newMigrations.Subtract(_migrationHistory);

            var sql = ImmutableList<string>.Empty;

            while (difference.Any)
            {
                var migrationsAffected = new MigrationHistoryBuilder();
                migrationsAffected.Append(difference.Head);
                string[] result = difference.Head.GenerateSql(migrationsAffected);
                sql = sql.AddRange(result);
                difference = difference.Subtract(migrationsAffected.MigrationHistory);
            }

            return sql.ToArray();
        }
    }
}