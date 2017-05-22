using MergableMigrations.EF6;
using MergableMigrations.Specification.Implementation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Numerics;

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
                        [MigrationId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [Type] VARCHAR(50) NOT NULL,
                        [HashCode] VARBINARY(32) NOT NULL,
                        [Attributes] NVARCHAR(MAX) NOT NULL,
	                    INDEX [IX_MergableMigration_HashCode] UNIQUE ([HashCode]))",
                    $@"CREATE TABLE [{DatabaseName}].[dbo].[__MergableMigrationHistoryPrerequisite] (
	                    [MigrationId] INT NOT NULL,
	                    [Role] NVARCHAR(50) NOT NULL,
	                    [PrerequisiteMigrationId] INT NOT NULL,
	                    INDEX [IX_MergableMigrationPrerequisite_MigrationId] ([MigrationId]),
	                    FOREIGN KEY ([MigrationId]) REFERENCES [Mathematicians].[dbo].[__MergableMigrationHistory],
	                    INDEX [IX_MergableMigrationPrerequisite_PrerequisiteMigrationId] ([PrerequisiteMigrationId]),
	                    FOREIGN KEY ([PrerequisiteMigrationId]) REFERENCES [Mathematicians].[dbo].[__MergableMigrationHistory])"
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
                var rows = ExecuteSqlQuery(Master,
                    $@"SELECT h.[Type], h.[HashCode], h.[Attributes], j.[Role], p.[HashCode] AS [PrerequisiteHashCode]
                        FROM [{DatabaseName}].[dbo].[__MergableMigrationHistory] h
                        LEFT JOIN [{DatabaseName}].[dbo].[__MergableMigrationHistoryPrerequisite] j
                          ON h.MigrationId = j.MigrationId
                        LEFT JOIN [{DatabaseName}].[dbo].[__MergableMigrationHistory] p
                          ON j.PrerequisiteMigrationId = p.MigrationId
                        ORDER BY h.MigrationId, j.Role, p.MigrationId",
                    row => new MigrationHistoryRow
                    {
                        Type = LoadString(row["Type"]),
                        HashCode = LoadBigInteger(row["HashCode"]),
                        Attributes = LoadString(row["Attributes"]),
                        Role = LoadString(row["Role"]),
                        PrerequisiteHashCode = LoadBigInteger(row["PrerequisiteHashCode"])
                    });

                return MigrationHistory.LoadMementos(LoadMementos(rows));
            }
            else
            {
                return new MigrationHistory();
            }
        }

        private static IEnumerable<MigrationMemento> LoadMementos(
            IEnumerable<MigrationHistoryRow> rows)
        {
            var enumerator = new LookaheadEnumerator<MigrationHistoryRow>(rows.GetEnumerator());
            if (enumerator.MoveNext())
            {
                do
                {
                    yield return LoadMemento(enumerator);
                } while (enumerator.More);
            }
        }

        private static MigrationMemento LoadMemento(LookaheadEnumerator<MigrationHistoryRow> enumerator)
        {
            var type = enumerator.Current.Type;
            var hashCode = enumerator.Current.HashCode;
            var attributes = enumerator.Current.Attributes;
            var roles = LoadRoles(hashCode, enumerator);

            var migrationAttributes = JsonConvert.DeserializeObject<Dictionary<string, string>>(attributes);
            var memento = new MigrationMemento(
                type,
                migrationAttributes,
                hashCode,
                roles);
            return memento;
        }

        private static IDictionary<string, IEnumerable<BigInteger>> LoadRoles(BigInteger hashCode, LookaheadEnumerator<MigrationHistoryRow> enumerator)
        {
            var result = new Dictionary<string, IEnumerable<BigInteger>>();
            do
            {
                string role = enumerator.Current.Role;
                if (role != null)
                {
                    var prerequisites = LoadPrerequisites(hashCode, role, enumerator).ToList();
                    result[role] = prerequisites;
                }
                else
                {
                    enumerator.MoveNext();
                }
            } while (enumerator.More && enumerator.Current.HashCode == hashCode);

            return result;
        }

        private static IEnumerable<BigInteger> LoadPrerequisites(BigInteger hashCode, string role, LookaheadEnumerator<MigrationHistoryRow> enumerator)
        {
            do
            {
                yield return enumerator.Current.PrerequisiteHashCode;
            } while (enumerator.MoveNext() && enumerator.Current.HashCode == hashCode && enumerator.Current.Role == role);
        }

        private static string LoadString(object value)
        {
            if (value is DBNull)
                return null;
            else
                return (string)value;
        }

        private static BigInteger LoadBigInteger(object value)
        {
            if (value is DBNull)
                return BigInteger.Zero;
            else
                return new BigInteger(((byte[])value).Reverse().ToArray());
        }

        public void DestroyDatabase()
        {
            var fileNames = ExecuteSqlQuery(Master, $@"
                SELECT [physical_name] FROM [sys].[master_files]
                WHERE [database_id] = DB_ID('{DatabaseName}')",
                row => (string)row["physical_name"]);

            if (fileNames.Any())
            {
                ExecuteSqlCommand(Master, $@"
                    ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    EXEC sp_detach_db '{DatabaseName}'");

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
            if (commands.Any())
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
