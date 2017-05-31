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
        private readonly string _defaultExpression;

        public string DatabaseName => _parent.DatabaseName;
        public string SchemaName => _parent.SchemaName;
        public string TableName => _parent.TableName;
        public string ColumnName => _columnName;
        public string TypeDescriptor => _typeDescriptor;
        public string DefaultExpression => _defaultExpression;
        internal override CreateTableMigration CreateTableMigration => _parent;

        public CreateColumnMigration(CreateTableMigration parent, string columnName, string typeDescriptor, string defaultExpression)
        {
            _parent = parent;
            _columnName = columnName;
            _typeDescriptor = typeDescriptor;
            _defaultExpression = defaultExpression;
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            if (DefaultExpression == null)
            {
                string[] sql =
                {
                    $@"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]
    ADD [{ColumnName}] {TypeDescriptor}"
                };

                return sql;
            }
            else
            {
                string[] sql =
                {
                    $@"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]
    ADD [{ColumnName}] {TypeDescriptor}
    CONSTRAINT [DF_{ColumnName}] DEFAULT ({DefaultExpression})",
                    $@"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]
    DROP CONSTRAINT [DF_{ColumnName}]",
                };

                return sql;
            }
        }

        public override string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected)
        {
            string[] sql =
            {
                $@"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]
    DROP COLUMN [{ColumnName}]"
            };

            return sql;
        }

        internal override string GenerateDefinitionSql()
        {
            return $"\r\n    [{ColumnName}] {TypeDescriptor}";
        }

        protected override BigInteger ComputeSha256Hash()
        {
            if (_defaultExpression == null)
            {
                return nameof(CreateColumnMigration).Sha256Hash().Concatenate(
                    _parent.Sha256Hash,
                    _columnName.Sha256Hash(),
                    _typeDescriptor.Sha256Hash());
            }
            else
            {
                return nameof(CreateColumnMigration).Sha256Hash().Concatenate(
                    _parent.Sha256Hash,
                    _columnName.Sha256Hash(),
                    _typeDescriptor.Sha256Hash(),
                    _defaultExpression.Sha256Hash());
            }
        }

        internal override MigrationMemento GetMemento()
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>
            {
                [nameof(ColumnName)] = ColumnName,
                [nameof(TypeDescriptor)] = TypeDescriptor
            };
            if (DefaultExpression != null)
                attributes[nameof(DefaultExpression)] = DefaultExpression;

            return new MigrationMemento(
                nameof(CreateColumnMigration),
                attributes,
                Sha256Hash,
                new Dictionary<string, IEnumerable<BigInteger>>
                {
                    ["Parent"] = new BigInteger[] { _parent.Sha256Hash }
                });
        }

        public static CreateColumnMigration FromMemento(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            string defaultExpression;
            if (!memento.Attributes.TryGetValue("DefaultExpression", out defaultExpression))
                defaultExpression = null;

            return new CreateColumnMigration(
                (CreateTableMigration)migrationsByHashCode[memento.Prerequisites["Parent"].Single()],
                memento.Attributes["ColumnName"],
                memento.Attributes["TypeDescriptor"],
                defaultExpression);
        }
    }
}
