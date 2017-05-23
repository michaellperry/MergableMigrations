using System.Numerics;

namespace MergableMigrations.EF6.Loader
{
    class MigrationHistoryRow
    {
        public string Attributes { get; set; }
        public BigInteger HashCode { get; set; }
        public BigInteger PrerequisiteHashCode { get; set; }
        public string Role { get; set; }
        public string Type { get; set; }
    }
}
