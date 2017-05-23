using System.Collections.Generic;

namespace MergableMigrations.EF6.Loader
{
    class MigrationBody
    {
        public Dictionary<string, string> Attributes { get; set; }
        public Dictionary<string, List<string>> Prerequisites { get; set; }
    }
}
