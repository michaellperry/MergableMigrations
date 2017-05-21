using FluentAssertions;
using MergableMigrations.EF6;
using MergableMigrations.Specification;
using MergableMigrations.Specification.Implementation;
using System.Linq;
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

        [Fact]
        public void CanSaveMigrationHistory()
        {
            var migrationHistory = GivenCompleteMigrationHistory(new Migrations());
            var mementos = migrationHistory.GetMementos().ToArray();

            mementos.Length.Should().Be(9);

            mementos[0].Type.Should().Be("CreateDatabaseMigration");
            mementos[0].Attributes["DatabaseName"].Should().Be("Mathematicians");
            mementos[0].HashCode.Should().Be(849757297);

            mementos[1].Type.Should().Be("UseSchemaMigration");
            mementos[1].Attributes["SchemaName"].Should().Be("dbo");
            mementos[1].Prerequisites.Should().Contain(849757297);
            mementos[1].HashCode.Should().Be(1290054232);

            mementos[2].Type.Should().Be("CreateTableMigration");
            mementos[2].Attributes["TableName"].Should().Be("Mathematician");
            mementos[2].Prerequisites.Should().Contain(1290054232);
            mementos[2].HashCode.Should().Be(-138479862);
        }

        private MigrationHistory GivenCompleteMigrationHistory(Migrations migrations)
        {
            var model = new ModelSpecification();
            migrations.AddMigrations(model);
            return model.MigrationHistory;
        }
    }
}
