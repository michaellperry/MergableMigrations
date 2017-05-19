using FluentAssertions;
using MergableMigrations.EF6;
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
            var sqlGenerator = new SqlGenerator(migrations);
            var sql = sqlGenerator.Generate();

            sql.Should().Be(@"
");
        }
    }
}
