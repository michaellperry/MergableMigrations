using System;
using MergableMigrations.Specification.Implementation;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Collections.Immutable;

namespace MergableMigrations.Specification.Migrations
{
    class CreateColumnMigration : TableDefinitionMigration
    {
        private readonly CreateTableMigration _parent;
        private readonly string _columnName;
        private readonly string _typeDescriptor;

        public string DatabaseName => _parent.DatabaseName;
        public string SchemaName => _parent.SchemaName;
        public string TableName => _parent.TableName;
        public string ColumnName => _columnName;
        public string TypeDescriptor => _typeDescriptor;
        internal override CreateTableMigration CreateTableMigration => _parent;

        public CreateColumnMigration(CreateTableMigration parent, string columnName, string typeDescriptor)
        {
            _parent = parent;
            _columnName = columnName;
            _typeDescriptor = typeDescriptor;
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string[] sql =
            {
                $"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]\r\n    ADD [{ColumnName}] {TypeDescriptor}"
            };

            return sql;
        }

        public override string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected)
        {
            string[] sql =
            {
                $"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]\r\n    DROP COLUMN [{ColumnName}]"
            };

            return sql;
        }

        internal override string GenerateDefinitionSql()
        {
            return $"\r\n    [{ColumnName}] {TypeDescriptor}";
        }

        protected override BigInteger ComputeSha256Hash()
        {
            return nameof(CreateColumnMigration).Sha256Hash().Concatenate(
                _parent.Sha256Hash,
                _columnName.Sha256Hash(),
                _typeDescriptor.Sha256Hash());
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(CreateColumnMigration),
                new Dictionary<string, string>
                {
                    [nameof(ColumnName)] = ColumnName,
                    [nameof(TypeDescriptor)] = TypeDescriptor
                },
                Sha256Hash,
                new Dictionary<string, IEnumerable<BigInteger>>
                {
                    ["Parent"] = new BigInteger[] { _parent.Sha256Hash }
                });
        }

        public static CreateColumnMigration FromMemento(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            return new CreateColumnMigration(
                (CreateTableMigration)migrationsByHashCode[memento.Prerequisites["Parent"].Single()],
                memento.Attributes["ColumnName"],
                memento.Attributes["TypeDescriptor"]);
        }
    }
}
