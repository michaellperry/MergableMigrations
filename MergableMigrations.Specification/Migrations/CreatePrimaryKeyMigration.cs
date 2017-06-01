using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification.Migrations
{
    class CreatePrimaryKeyMigration : IndexMigration
    {
        private readonly CreateTableMigration _parent;
        private readonly ImmutableList<CreateColumnMigration> _columns;

        public override string DatabaseName => _parent.DatabaseName;
        public override string SchemaName => _parent.SchemaName;
        public override string TableName => _parent.TableName;
        public override IEnumerable<CreateColumnMigration> Columns => _columns;
        internal override CreateTableMigration CreateTableMigration => _parent;

        public CreatePrimaryKeyMigration(CreateTableMigration parent, IEnumerable<CreateColumnMigration> columns, ImmutableList<Migration> prerequisites) :
            base(prerequisites)
        {
            _parent = parent;
            _columns = columns.ToImmutableList();
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string columnNames = string.Join(", ", _columns.Select(c => $"[{c.ColumnName}]").ToArray());
            string[] sql =
            {
                $"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]\r\n    ADD CONSTRAINT [PK_{TableName}] PRIMARY KEY CLUSTERED ({columnNames})"
            };

            return sql;
        }

        public override string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected)
        {
            string[] sql =
            {
                $"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]\r\n    DROP CONSTRAINT [PK_{TableName}]"
            };

            return sql;
        }

        internal override string GenerateDefinitionSql()
        {
            string columnNames = string.Join(", ", _columns.Select(c => $"[{c.ColumnName}]").ToArray());

            return $"\r\n    CONSTRAINT [PK_{TableName}] PRIMARY KEY CLUSTERED ({columnNames})";
        }

        protected override BigInteger ComputeSha256Hash()
        {
            return nameof(CreatePrimaryKeyMigration).Sha256Hash().Concatenate(
                Enumerable.Repeat(_parent.Sha256Hash, 1)
                    .Concat(_columns.Select(c => c.Sha256Hash))
                    .ToArray());
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(CreatePrimaryKeyMigration),
                new Dictionary<string, string>
                {
                },
                Sha256Hash,
                new Dictionary<string, IEnumerable<BigInteger>>
                {
                    ["Prerequisites"] = Prerequisites.Select(x => x.Sha256Hash),
                    ["Parent"] = new[] { _parent.Sha256Hash },
                    ["Columns"] = _columns.Select(c => c.Sha256Hash).ToArray()
                });
        }

        public static CreatePrimaryKeyMigration FromMemento(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            return new CreatePrimaryKeyMigration(
                (CreateTableMigration)migrationsByHashCode[memento.Prerequisites["Parent"].Single()],
                memento.Prerequisites["Columns"].Select(p => migrationsByHashCode[p]).OfType<CreateColumnMigration>(),
                memento.Prerequisites["Prerequisites"].Select(p => migrationsByHashCode[p]).ToImmutableList());
        }
    }
}