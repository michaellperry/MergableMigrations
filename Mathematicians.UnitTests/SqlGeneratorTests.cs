﻿using FluentAssertions;
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
            var sql = WhenGenerateSql(migrations, migrationHistory);

            sql.Should().Contain(@"CREATE TABLE [Mathematicians].[dbo].[Mathematician](
    [MathematicianId] INT NOT NULL,
    [BirthYear] INT NOT NULL,
    [DeathYear] INT NULL)");
            sql.Should().Contain(@"CREATE TABLE [Mathematicians].[dbo].[Contribution](
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

            mementos.Length.Should().Be(8);

            mementos[0].Type.Should().Be("UseSchemaMigration");
            mementos[0].Attributes["SchemaName"].Should().Be("dbo");

            mementos[1].Type.Should().Be("CreateTableMigration");
            mementos[1].Attributes["TableName"].Should().Be("Mathematician");
            mementos[1].Prerequisites["Parent"].Should().Contain(mementos[0].HashCode);
        }

        [Fact]
        public void CanUpgradeToANewVersion()
        {
            var previousVersion = GivenMigrationMementos(new Migrations());
            var migrationHistory = WhenLoadMigrationHistory(previousVersion);
            var sql = WhenGenerateSql(new MigrationsV2(), migrationHistory);

            sql.Should().Contain(@"CREATE TABLE [Mathematicians].[dbo].[Field](
    [FieldId] INT NOT NULL)");
            sql.Should().Contain(@"ALTER TABLE [Mathematicians].[dbo].[Contribution]
    ADD [FieldId] INT NOT NULL");
        }

        private MigrationMemento[] GivenMigrationMementos(IMigrations migrations)
        {
            var migrationHistory = GivenCompleteMigrationHistory(migrations);
            return migrationHistory.GetMementos().ToArray();
        }

        private MigrationHistory GivenCompleteMigrationHistory(IMigrations migrations)
        {
            var databaseSpecification = new DatabaseSpecification("Mathematicians");
            migrations.AddMigrations(databaseSpecification);
            return databaseSpecification.MigrationHistory;
        }

        private MigrationHistory WhenLoadMigrationHistory(MigrationMemento[] mementos)
        {
            return MigrationHistory.LoadMementos(mementos);
        }

        private static string[] WhenGenerateSql(IMigrations migrations, MigrationHistory migrationHistory)
        {
            var sqlGenerator = new SqlGenerator(migrations, migrationHistory);
            return sqlGenerator.Generate("Mathematicians");
        }
    }
}
