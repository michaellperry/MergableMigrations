using System;
using System.Numerics;

namespace Mathematicians.Web.Data
{
    public class MigrationHistoryRow
    {
        public MigrationHistoryRow()
        {
        }
        public string Attributes { get; set; }
        public BigInteger HashCode { get; set; }
        public BigInteger PrerequisiteHashCode { get; set; }
        public string Role { get; set; }
        public string Type { get; set; }
    }
}
