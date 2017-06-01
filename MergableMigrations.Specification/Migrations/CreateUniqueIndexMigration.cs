using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using MergableMigrations.Specification.Implementation;

namespace MergableMigrations.Specification.Migrations
{
    class CreateUniqueIndexMigration : IndexMigration
    {
        private readonly CreateTableMigration _parent;
        private readonly ImmutableList<CreateColumnMigration> _columns;

        public override string DatabaseName => _parent.DatabaseName;
        public override string SchemaName => _parent.SchemaName;
        public override string TableName => _parent.TableName;
        public override IEnumerable<CreateColumnMigration> Columns => _columns;
        internal override CreateTableMigration CreateTableMigration => _parent;

        public CreateUniqueIndexMigration(CreateTableMigration parent, IEnumerable<CreateColumnMigration> columns, ImmutableList<Migration> prerequsites) :
            base(prerequsites)
        {
            _parent = parent;
            _columns = columns.ToImmutableList();
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected)
        {
            string indexTail = string.Join("_", Columns.Select(c => $"{c.ColumnName}").ToArray());
            string columnList = string.Join(", ", Columns.Select(c => $"[{c.ColumnName}]").ToArray());
            string[] sql =
            {
                $"CREATE UNIQUE NONCLUSTERED INDEX [UX_{TableName}_{indexTail}] ON [{DatabaseName}].[{SchemaName}].[{TableName}] ({columnList})"
            };

            return sql;
        }

        public override string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected)
        {
            string indexTail = string.Join("_", Columns.Select(c => $"{c.ColumnName}").ToArray());
            string[] sql =
            {
                $"DROP INDEX [UX_{TableName}_{indexTail}] ON [{DatabaseName}].[{SchemaName}].[{TableName}]"
            };

            return sql;
        }

        internal override string GenerateDefinitionSql()
        {
            string indexTail = string.Join("_", Columns.Select(c => $"{c.ColumnName}").ToArray());
            string columnList = string.Join(", ", Columns.Select(c => $"[{c.ColumnName}]").ToArray());

            return $"\r\n    INDEX [UX_{TableName}_{indexTail}] UNIQUE NONCLUSTERED ({columnList})";
        }

        protected override BigInteger ComputeSha256Hash()
        {
            return nameof(CreateUniqueIndexMigration).Sha256Hash().Concatenate(
                Enumerable.Repeat(_parent.Sha256Hash, 1)
                    .Concat(_columns.Select(c => c.Sha256Hash))
                    .ToArray());
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(CreateUniqueIndexMigration),
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

        public static CreateUniqueIndexMigration FromMemento(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            return new CreateUniqueIndexMigration(
                (CreateTableMigration)migrationsByHashCode[memento.Prerequisites["Parent"].Single()],
                memento.Prerequisites["Columns"].Select(p => migrationsByHashCode[p]).OfType<CreateColumnMigration>(),
                memento.Prerequisites["Prerequisites"].Select(x => migrationsByHashCode[x]).ToImmutableList());
        }
    }
}
