using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification.Migrations
{
    class CreateTableMigration : Migration
    {
        private readonly UseSchemaMigration _parent;
        private readonly string _tableName;

        private ImmutableList<TableDefinitionMigration> _definitions =
            ImmutableList<TableDefinitionMigration>.Empty;

        public string DatabaseName => _parent.DatabaseName;
        public string SchemaName => _parent.SchemaName;
        public string TableName => _tableName;

        public CreateTableMigration(UseSchemaMigration parent, string tableName)
        {
            _parent = parent;
            _tableName = tableName;
        }

        internal void AddDefinition(TableDefinitionMigration childMigration)
        {
            _definitions = _definitions.Add(childMigration);
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string createTable;
            string head = $"CREATE TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]";
            if (_definitions.Any())
            {
                var definitions = _definitions.Select(d => d.GenerateDefinitionSql());
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
            migrationsAffected.AppendAll(_definitions);

            return sql;
        }

        public override string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected)
        {
            string[] sql =
            {
                $"DROP TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]"
            };
            migrationsAffected.AppendAll(_definitions);

            return sql;
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