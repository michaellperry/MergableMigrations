using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;

namespace MergableMigrations.Specification.Implementation
{
    class CreateTableMigration : Migration
    {
        private readonly UseSchemaMigration _parent;
        private readonly string _tableName;

        private ImmutableList<CreateColumnMigration> _columns =
            ImmutableList<CreateColumnMigration>.Empty;
        private ImmutableList<PrimaryKeyMigration> _primaryKeys =
            ImmutableList<PrimaryKeyMigration>.Empty;

        public string DatabaseName => _parent.DatabaseName;
        public string SchemaName => _parent.SchemaName;
        public string TableName => _tableName;

        public CreateTableMigration(UseSchemaMigration parent, string tableName)
        {
            _parent = parent;
            _tableName = tableName;
        }

        internal void AddColumn(CreateColumnMigration childMigration)
        {
            _columns = _columns.Add(childMigration);
        }

        internal void AddPrimaryKey(PrimaryKeyMigration childMigration)
        {
            _primaryKeys = _primaryKeys.Add(childMigration);
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string createTable;
            string head = $"CREATE TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]";
            if (_columns.Any())
            {
                var definitions = _columns.Select(GenerateColumnSql)
                    .Concat(_primaryKeys.Select(GeneratePrimaryKeySql));
                createTable = $"{head}({string.Join(",", definitions)})";
            }
            else
            {
                createTable = head;
            }

            string[] sql =
            {
                createTable
            };
            migrationsAffected.AppendAll(_columns);
            migrationsAffected.AppendAll(_primaryKeys);

            return sql;
        }

        private string GenerateColumnSql(CreateColumnMigration column)
        {
            return $"\r\n    [{column.ColumnName}] {column.TypeDescriptor}";
        }

        private string GeneratePrimaryKeySql(PrimaryKeyMigration primaryKey)
        {
            string columnNames = string.Join(", ", primaryKey.Columns.Select(c => $"[{c.ColumnName}]").ToArray());

            return $"\r\n    CONSTRAINT [PK_{TableName}] PRIMARY KEY CLUSTERED ({columnNames})";
        }

        protected override BigInteger ComputeSha256Hash()
        {
            return nameof(CreateTableMigration).Sha256Hash().Concatenate(
                _parent.Sha256Hash,
                _tableName.Sha256Hash());
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(CreateTableMigration),
                new Dictionary<string, string>
                {
                    [nameof(TableName)] = TableName
                },
                Sha256Hash,
                new Dictionary<string, IEnumerable<BigInteger>>
                {
                    ["Parent"] = new BigInteger[] { _parent.Sha256Hash }
                });
        }

        public static CreateTableMigration FromMemento(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            return new CreateTableMigration(
                (UseSchemaMigration)migrationsByHashCode[memento.Prerequisites["Parent"].Single()],
                memento.Attributes["TableName"]);
        }
    }
}