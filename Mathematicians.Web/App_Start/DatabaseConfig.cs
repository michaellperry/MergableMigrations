using Mathematicians.Web.Data;
using System;

namespace Mathematicians.Web.App_Start
{
    class DatabaseConfig
    {
        public static void Configure(string fileName)
        {
            var setup = new Setup(fileName);
            setup.MigrateDatabase();
        }
    }
}