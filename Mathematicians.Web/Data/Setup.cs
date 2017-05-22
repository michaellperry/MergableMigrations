using MergableMigrations.EF6;
using MergableMigrations.Specification.Implementation;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Mathematicians.Web.Data
{
    public class Setup
    {
        private const string DatabaseName = "Mathematicians";

        private string _fileName;

        public Setup(string fileName)
        {
            _fileName = fileName;
        }

        public void MigrateDatabase()
        {
            var migrationHistory = LoadMigrationHistory();

            if (migrationHistory.Empty)
            {
                string[] initialize =
                {
                    $@"CREATE DATABASE [{DatabaseName}]
                        ON (NAME = '{DatabaseName}',
                        FILENAME = '{_fileName}')",
                    $@"CREATE TABLE [{DatabaseName}].[dbo].[__MergableMigrationHistory](
                        [MigrationId] INT IDENTITY(1,1) NOT NULL,
                        [Type] VARCHAR(50) NOT NULL,
                        [HashCode] VARBINARY(32) NOT NULL,
                        [Body] NVARCHAR(MAX) NOT NULL)"
                };
                ExecuteSqlCommands(Master, initialize);
            }

            var generator = new SqlGenerator(new Migrations(), migrationHistory);

            var sql = generator.Generate();
            ExecuteSqlCommands(Master, sql);
        }

        private static MigrationHistory LoadMigrationHistory()
        {
            var ids = ExecuteSqlQuery(Master,
                $"SELECT database_id FROM master.sys.databases WHERE name = '{DatabaseName}'",
                row => (int)row["database_id"]);
            if (ids.Any())
            {
                var mementos = ExecuteSqlQuery(Master,
                    $"SELECT Type FROM [{DatabaseName}].[dbo].[__MergableMigrationHistory]",
                    row => LoadMigrationMemento(
                        (string)row["Type"],
                        (byte[])row["HashCode"],
                        (string)row["Body"]));
                return MigrationHistory.LoadMementos(mementos);
            }
            else
            {
                return new MigrationHistory();
            }
        }

        private static MigrationMemento LoadMigrationMemento(string type, byte[] hashCode, string body)
        {
            var attributes = (IDictionary<string, string>)null;
            var prerequisites = (IDictionary<string, IEnumerable<System.Numerics.BigInteger>>)null;
            var memento = new MigrationMemento(
                type,
                attributes,
                new System.Numerics.BigInteger(hashCode),
                prerequisites);
            return memento;
        }

        public void DestroyDatabase()
        {
            var fileNames = ExecuteSqlQuery(Master, @"
                SELECT [physical_name] FROM [sys].[master_files]
                WHERE [database_id] = DB_ID('Globalmantics')",
                row => (string)row["physical_name"]);

            if (fileNames.Any())
            {
                ExecuteSqlCommand(Master, @"
                    ALTER DATABASE [Globalmantics] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    EXEC sp_detach_db 'Globalmantics'");

                fileNames.ForEach(File.Delete);
            }
        }

        private static void ExecuteSqlCommand(
            SqlConnectionStringBuilder connectionStringBuilder,
            string commandText)
        {
            using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void ExecuteSqlCommands(
            SqlConnectionStringBuilder connectionStringBuilder,
            IEnumerable<string> commands)
        {
            using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                foreach (var commandText in commands)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static List<T> ExecuteSqlQuery<T>(
            SqlConnectionStringBuilder connectionStringBuilder,
            string queryText,
            Func<SqlDataReader, T> read)
        {
            var result = new List<T>();
            using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(read(reader));
                        }
                    }
                }
            }
            return result;
        }

        private static SqlConnectionStringBuilder Master =>
            new SqlConnectionStringBuilder
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                InitialCatalog = "master",
                IntegratedSecurity = true
            };
    }
}
