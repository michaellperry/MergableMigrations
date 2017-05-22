using System;
using System.Collections.Generic;

namespace MergableMigrations.EF6
{
    public class MigrationBody
    {
        public Dictionary<string, string> Attributes { get; set; }
        public Dictionary<string, List<string>> Prerequisites { get; set; }
    }
}
