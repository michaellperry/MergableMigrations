using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MergableMigrations.EF6;
using FluentAssertions;

namespace Mathematicians.UnitTests
{
    [TestClass]
    public class SqlGenerationTests
    {
        [TestMethod]
        public void GeneratesSql()
        {
            var migrations = new Migrations();
            var sqlGenerator = new SqlGenerator(migrations);
            var sql = sqlGenerator.Generate();

            sql.Should().Be(@"
");
        }
    }
}
