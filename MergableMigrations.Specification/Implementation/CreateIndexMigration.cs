using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;

namespace MergableMigrations.Specification.Implementation
{
    class CreateIndexMigration : IndexMigration
    {
        private readonly CreateTableMigration _parent;
        private readonly ImmutableList<CreateColumnMigration> _columns;

        public override string DatabaseName => _parent.DatabaseName;
        public override string SchemaName => _parent.SchemaName;
        public override string TableName => _parent.TableName;
        public override IEnumerable<CreateColumnMigration> Columns => _columns;

        public CreateIndexMigration(CreateTableMigration parent, IEnumerable<CreateColumnMigration> columns)
        {
            _parent = parent;
            _columns = columns.ToImmutableList();
        }

        internal void AddDefinition(CreateForeignKeyMigration childMigration)
        {
            _parent.AddDefinition(childMigration);
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string indexTail = string.Join("_", Columns.Select(c => $"{c.ColumnName}").ToArray());
            string columnList = string.Join(", ", Columns.Select(c => $"[{c.ColumnName}]").ToArray());
            string[] sql =
            {
                $"CREATE NONCLUSTERED INDEX [IX_{TableName}_{indexTail}] ON [{DatabaseName}].[{SchemaName}].[{TableName}] ({columnList})"
            };

            return sql;
        }

        public override string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected)
        {
            string indexTail = string.Join("_", Columns.Select(c => $"{c.ColumnName}").ToArray());
            string[] sql =
            {
                $"DROP INDEX [IX_{TableName}_{indexTail}]"
            };

            return sql;
        }

        internal override string GenerateDefinitionSql()
        {
            string indexTail = string.Join("_", _columns.Select(c => $"{c.ColumnName}").ToArray());
            string columnList = string.Join(", ", _columns.Select(c => $"[{c.ColumnName}]").ToArray());

            return $"\r\n    INDEX [IX_{TableName}_{indexTail}] NONCLUSTERED ({columnList})";
        }

        protected override BigInteger ComputeSha256Hash()
        {
            return nameof(CreateIndexMigration).Sha256Hash().Concatenate(
                Enumerable.Repeat(_parent.Sha256Hash, 1)
                    .Concat(_columns.Select(c => c.Sha256Hash))
                    .ToArray());
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(CreateIndexMigration),
                new Dictionary<string, string>
                {
                },
                Sha256Hash,
                new Dictionary<string, IEnumerable<BigInteger>>
                {
                    ["Parent"] = new BigInteger[] { _parent.Sha256Hash },
                    ["Columns"] = _columns.Select(c => c.Sha256Hash).ToArray()
                });
        }

        public static CreateIndexMigration FromMemento(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            return new CreateIndexMigration(
                (CreateTableMigration)migrationsByHashCode[memento.Prerequisites["Parent"].Single()],
                memento.Prerequisites["Columns"].Select(p => migrationsByHashCode[p]).OfType<CreateColumnMigration>());
        }
    }
}
