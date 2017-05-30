using MergableMigrations.EF6.Loader;
using MergableMigrations.Specification;
using MergableMigrations.Specification.Implementation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Numerics;

namespace MergableMigrations.EF6
{
    public class DatabaseMigrator
    {
        private readonly string _databaseName;
        private readonly string _fileName;
        private readonly string _masterConnectionString;
        private readonly IMigrations _migrations;

        public DatabaseMigrator(string databaseName, string fileName, string masterConnectionString, IMigrations migrations)
        {
            _databaseName = databaseName;
            _fileName = fileName;
            _masterConnectionString = masterConnectionString;
            _migrations = migrations;
        }

        public void MigrateDatabase()
        {
            var migrationHistory = LoadMigrationHistory();

            if (migrationHistory.Empty)
            {
                string[] initialize =
                {
                    $@"CREATE DATABASE [{_databaseName}]
                        ON (NAME = '{_databaseName}',
                        FILENAME = '{_fileName}')",
                    $@"CREATE TABLE [{_databaseName}].[dbo].[__MergableMigrationHistory](
                        [MigrationId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [Type] VARCHAR(50) NOT NULL,
                        [HashCode] VARBINARY(32) NOT NULL,
                        [Attributes] NVARCHAR(MAX) NOT NULL,
	                    INDEX [IX_MergableMigration_HashCode] UNIQUE ([HashCode]))",
                    $@"CREATE TABLE [{_databaseName}].[dbo].[__MergableMigrationHistoryPrerequisite] (
	                    [MigrationId] INT NOT NULL,
	                    [Role] NVARCHAR(50) NOT NULL,
	                    [PrerequisiteMigrationId] INT NOT NULL,
	                    INDEX [IX_MergableMigrationPrerequisite_MigrationId] ([MigrationId]),
	                    FOREIGN KEY ([MigrationId]) REFERENCES [Mathematicians].[dbo].[__MergableMigrationHistory],
	                    INDEX [IX_MergableMigrationPrerequisite_PrerequisiteMigrationId] ([PrerequisiteMigrationId]),
	                    FOREIGN KEY ([PrerequisiteMigrationId]) REFERENCES [Mathematicians].[dbo].[__MergableMigrationHistory])"
                };
                ExecuteSqlCommands(initialize);
            }

            var generator = new SqlGenerator(_migrations, migrationHistory);

            var sql = generator.Generate(_databaseName);
            ExecuteSqlCommands(sql);
        }

        public void RollbackDatabase()
        {
            var migrationHistory = LoadMigrationHistory();
            var generator = new SqlGenerator(_migrations, migrationHistory);
            var sql = generator.GenerateRollbackSql(_databaseName);

            ExecuteSqlCommands(sql);
        }

        public void DestroyDatabase()
        {
            var fileNames = ExecuteSqlQuery($@"
                SELECT [physical_name] FROM [sys].[master_files]
                WHERE [database_id] = DB_ID('{_databaseName}')",
                row => (string)row["physical_name"]);

            if (fileNames.Any())
            {
                ExecuteSqlCommand($@"
                    ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    EXEC sp_detach_db '{_databaseName}'");

                fileNames.ForEach(File.Delete);
            }
        }

        private MigrationHistory LoadMigrationHistory()
        {
            var ids = ExecuteSqlQuery($"SELECT database_id FROM master.sys.databases WHERE name = '{_databaseName}'",
                row => (int)row["database_id"]);
            if (ids.Any())
            {
                var rows = ExecuteSqlQuery($@"SELECT h.[Type], h.[HashCode], h.[Attributes], j.[Role], p.[HashCode] AS [PrerequisiteHashCode]
                        FROM [{_databaseName}].[dbo].[__MergableMigrationHistory] h
                        LEFT JOIN [{_databaseName}].[dbo].[__MergableMigrationHistoryPrerequisite] j
                          ON h.MigrationId = j.MigrationId
                        LEFT JOIN [{_databaseName}].[dbo].[__MergableMigrationHistory] p
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
            enumerator.MoveNext();
            if (enumerator.More)
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
                enumerator.MoveNext();
            } while (enumerator.More && enumerator.Current.HashCode == hashCode && enumerator.Current.Role == role);
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

        private void ExecuteSqlCommand(string commandText)
        {
            using (var connection = new SqlConnection(_masterConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }

        private void ExecuteSqlCommands(IEnumerable<string> commands)
        {
            if (commands.Any())
            {
                using (var connection = new SqlConnection(_masterConnectionString))
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

        private List<T> ExecuteSqlQuery<T>(
            string queryText,
            Func<SqlDataReader, T> read)
        {
            var result = new List<T>();
            using (var connection = new SqlConnection(_masterConnectionString))
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
    }
}
