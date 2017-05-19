using FluentAssertions;
using MergableMigrations.EF6;
using MergableMigrations.Specification.Implementation;
using System;
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
            var migrationHistory = MigrationHistory.Empty;
            var sqlGenerator = new SqlGenerator(migrations, migrationHistory);
            var sql = sqlGenerator.Generate();

            sql.Length.Should().Be(1);
            sql[0].Should().Be(@"
CREATE DATABASE [Mathematicians]
");
        }
    }
}
