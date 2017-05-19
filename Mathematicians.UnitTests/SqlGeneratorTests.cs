using FluentAssertions;
using MergableMigrations.EF6;
using MergableMigrations.Specification;
using MergableMigrations.Specification.Implementation;
using Xunit;

namespace Mathematicians.UnitTests
{
    public class SqlGeneratorTests
    {
        [Fact]
        public void CanGenerateSql()
        {
            var migrations = new Migrations();
            var migrationHistory = new MigrationHistory();
            var sqlGenerator = new SqlGenerator(migrations, migrationHistory);
            var sql = sqlGenerator.Generate();

            sql.Length.Should().Be(3);
            sql[0].Should().Be("CREATE DATABASE [Mathematicians]");
            sql[1].Should().Be(@"CREATE TABLE [Mathematicians].[dbo].[Mathematician](
    [MathematicianId] INT NOT NULL,
    [BirthYear] INT NOT NULL,
    [DeathYear] INT NULL)");
            sql[2].Should().Be(@"CREATE TABLE [Mathematicians].[dbo].[Contribution](
    [ContributionId] INT NOT NULL,
    [MathematicianId] INT NOT NULL)");
        }

        [Fact]
        public void GeneratesNoSqlWhenUpToDate()
        {
            var migrations = new Migrations();
            var migrationHistory = GivenCompleteMigrationHistory(migrations);
            var sqlGenerator = new SqlGenerator(migrations, migrationHistory);
            var sql = sqlGenerator.Generate();

            sql.Length.Should().Be(0);
        }

        private MigrationHistory GivenCompleteMigrationHistory(Migrations migrations)
        {
            var model = new ModelSpecification();
            migrations.AddMigrations(model);
            return model.MigrationHistory;
        }
    }
}
