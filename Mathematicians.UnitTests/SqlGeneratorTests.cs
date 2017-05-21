using FluentAssertions;
using MergableMigrations.EF6;
using MergableMigrations.Specification;
using MergableMigrations.Specification.Implementation;
using System.Linq;
using Xunit;
using System;

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
            var sql = WhenGenerateSql(migrations, migrationHistory);

            sql.Length.Should().Be(0);
        }

        [Fact]
        public void CanSaveMigrationHistory()
        {
            var mementos = GivenMigrationMementos(new Migrations());

            mementos.Length.Should().Be(9);

            mementos[0].Type.Should().Be("CreateDatabaseMigration");
            mementos[0].Attributes["DatabaseName"].Should().Be("Mathematicians");

            mementos[1].Type.Should().Be("UseSchemaMigration");
            mementos[1].Attributes["SchemaName"].Should().Be("dbo");
            mementos[1].Prerequisites.Should().Contain(mementos[0].HashCode);

            mementos[2].Type.Should().Be("CreateTableMigration");
            mementos[2].Attributes["TableName"].Should().Be("Mathematician");
            mementos[2].Prerequisites.Should().Contain(mementos[1].HashCode);
        }

        [Fact]
        public void CanUpgradeToANewVersion()
        {
            var previousVersion = GivenMigrationMementos(new Migrations());
            MigrationHistory migrationHistory = WhenLoadMigrationHistory(previousVersion);
            var sql = WhenGenerateSql(new MigrationsV2(), migrationHistory);

            sql.Length.Should().Be(2);
            sql[0].Should().Be(@"CREATE TABLE [Mathematicians].[dbo].[Field](
    [FieldId] INT NOT NULL)");
            sql[1].Should().Be(@"ALTER TABLE [Mathematicians].[dbo].[Contribution]
    ADD [FieldId] INT NOT NULL");
        }

        private MigrationMemento[] GivenMigrationMementos(IMigrations migrations)
        {
            var migrationHistory = GivenCompleteMigrationHistory(migrations);
            return migrationHistory.GetMementos().ToArray();
        }

        private MigrationHistory GivenCompleteMigrationHistory(IMigrations migrations)
        {
            var model = new ModelSpecification();
            migrations.AddMigrations(model);
            return model.MigrationHistory;
        }

        private MigrationHistory WhenLoadMigrationHistory(MigrationMemento[] mementos)
        {
            return MigrationHistory.LoadMementos(mementos);
        }

        private static string[] WhenGenerateSql(IMigrations migrations, MigrationHistory migrationHistory)
        {
            var sqlGenerator = new SqlGenerator(migrations, migrationHistory);
            return sqlGenerator.Generate();
        }
    }
}
