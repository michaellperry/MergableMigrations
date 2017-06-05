using System;
using Schemavolution.Specification.Implementation;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Collections.Immutable;

namespace Schemavolution.Specification.Migrations
{
    class CreateColumnMigration : TableDefinitionMigration
    {
        private readonly CreateTableMigration _parent;
        private readonly string _columnName;
        private readonly string _typeDescriptor;
        private readonly bool _nullable;

        public string DatabaseName => _parent.DatabaseName;
        public string SchemaName => _parent.SchemaName;
        public string TableName => _parent.TableName;
        public string ColumnName => _columnName;
        public string TypeDescriptor => _typeDescriptor;
        public bool Nullable => _nullable;
        internal override CreateTableMigration CreateTableMigration => _parent;

        public CreateColumnMigration(CreateTableMigration parent, string columnName, string typeDescriptor, bool nullable, ImmutableList<Migration> prerequsites) :
            base(prerequsites)
        {
            _parent = parent;
            _columnName = columnName;
            _typeDescriptor = typeDescriptor;
            _nullable = nullable;
        }

        public override IEnumerable<Migration> AllPrerequisites => Prerequisites
            .Concat(new[] { CreateTableMigration });

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected, IGraphVisitor graph)
        {
            string[] identityTypes = { "INT IDENTITY" };
            string[] numericTypes = { "BIGINT", "INT", "SMALLINT", "TINYINT", "MONEY", "SMALLMONEY", "DECIMAL", "FLOAT", "REAL" };
            string[] dateTypes = { "DATETIME", "SMALLDATETIME", "DATETIME2", "TIME" };
            string[] dateTimeOffsetTypes = { "DATETIMEOFFSET" };
            string[] stringTypes = { "NVARCHAR", "NCHAR", "NTEXT" };
            string[] asciiStringTypes = { "VARCHAR", "CHAR", "TEXT" };
            string[] guidTypes = { "UNIQUEIDENTIFIER" };

            string defaultExpression =
                _nullable ? null :
                identityTypes.Any(t => TypeDescriptor.StartsWith(t)) ? null :
                numericTypes.Any(t => TypeDescriptor.StartsWith(t)) ? "0" :
                dateTypes.Any(t => TypeDescriptor.StartsWith(t)) ? "GETUTCDATE()" :
                dateTimeOffsetTypes.Any(t => TypeDescriptor.StartsWith(t)) ? "SYSDATETIMEOFFSET()" :
                stringTypes.Any(t => TypeDescriptor.StartsWith(t)) ? "N''" :
                asciiStringTypes.Any(t => TypeDescriptor.StartsWith(t)) ? "''" :
                guidTypes.Any(t => TypeDescriptor.StartsWith(t)) ? "'00000000-0000-0000-0000-000000000000'" :
                null;
            if (defaultExpression == null)
            {
                string[] sql =
                {
                    $@"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]
    ADD [{ColumnName}] {TypeDescriptor} {NullableClause}"
                };

                return sql;
            }
            else
            {
                string[] sql =
                {
                    $@"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]
    ADD [{ColumnName}] {TypeDescriptor} {NullableClause}
    CONSTRAINT [DF_{TableName}_{ColumnName}] DEFAULT ({defaultExpression})",
                    $@"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]
    DROP CONSTRAINT [DF_{TableName}_{ColumnName}]",
                };

                return sql;
            }
        }

        public override string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected, IGraphVisitor graph)
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
            return $"\r\n    [{ColumnName}] {TypeDescriptor} {NullableClause}";
        }

        private string NullableClause => $"{(Nullable ? "NULL" : "NOT NULL")}";

        protected override BigInteger ComputeSha256Hash()
        {
            return nameof(CreateColumnMigration).Sha256Hash().Concatenate(
                _parent.Sha256Hash,
                _columnName.Sha256Hash(),
                _typeDescriptor.Sha256Hash(),
                _nullable ? "true".Sha256Hash() : "false".Sha256Hash());
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(CreateColumnMigration),
                new Dictionary<string, string>
                {
                    [nameof(ColumnName)] = ColumnName,
                    [nameof(TypeDescriptor)] = TypeDescriptor,
                    [nameof(Nullable)] = Nullable ? "true" : "false"
                },
                Sha256Hash,
                new Dictionary<string, IEnumerable<BigInteger>>
                {
                    ["Prerequisites"] = Prerequisites.Select(x => x.Sha256Hash),
                    ["Parent"] = new[] { _parent.Sha256Hash }
                });
        }

        public static CreateColumnMigration FromMemento(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            return new CreateColumnMigration(
                (CreateTableMigration)migrationsByHashCode[memento.Prerequisites["Parent"].Single()],
                memento.Attributes["ColumnName"],
                memento.Attributes["TypeDescriptor"],
                memento.Attributes["Nullable"] == "true",
                memento.Prerequisites["Prerequisites"].Select(x => migrationsByHashCode[x]).ToImmutableList());
        }
    }
}
