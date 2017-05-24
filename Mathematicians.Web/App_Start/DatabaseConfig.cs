using MergableMigrations.EF6;
using System.Data.SqlClient;
using System.Web;

namespace Mathematicians.Web.App_Start
{
    class DatabaseConfig
    {
        public static void Configure(HttpServerUtility server)
        {
            var master = new SqlConnectionStringBuilder
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                InitialCatalog = "master",
                IntegratedSecurity = true
            };

            string fileName = server.MapPath("~/App_Data/Mathematicians.mdf");
            string databaseName = "Mathematicians";
            var migrator = new DatabaseMigrator(
                databaseName,
                fileName,
                master.ConnectionString,
                new Migrations());
            migrator.MigrateDatabase();
        }
    }
}