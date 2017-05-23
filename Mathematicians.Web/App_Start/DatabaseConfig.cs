using MergableMigrations.EF6;

namespace Mathematicians.Web.App_Start
{
    class DatabaseConfig
    {
        public static void Configure(string fileName)
        {
            var migrator = new DatabaseMigrator("Mathematicians", fileName, new Migrations());
            migrator.MigrateDatabase();
        }
    }
}