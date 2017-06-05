using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Schemavolution.Specification.Implementation;

namespace Schemavolution.Specification.Migrations
{
    class CreateForeignKeyMigration : TableDefinitionMigration
    {
        private readonly IndexMigration _parent;
        private readonly CreatePrimaryKeyMigration _referencing;
        private readonly bool _cascadeDelete;
        private readonly bool _cascadeUpdate;

        public string DatabaseName => _parent.DatabaseName;
        public string SchemaName => _parent.SchemaName;
        public string TableName => _parent.TableName;
        public IEnumerable<CreateColumnMigration> Columns => _parent.Columns;
        internal override CreateTableMigration CreateTableMigration => _parent.CreateTableMigration;

        public CreateForeignKeyMigration(IndexMigration parent, CreatePrimaryKeyMigration referencing, bool cascadeDelete, bool cascadeUpdate, ImmutableList<Migration> prerequsites) :
            base(prerequsites)
        {
            _parent = parent;
            _referencing = referencing;
            _cascadeDelete = cascadeDelete;
            _cascadeUpdate = cascadeUpdate;
        }

        public override IEnumerable<Migration> AllPrerequisites => Prerequisites
            .Concat(new[] { CreateTableMigration });

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected, IGraphVisitor graph)
        {
            string indexTail = string.Join("_", Columns.Select(c => $"{c.ColumnName}").ToArray());
            string columnList = string.Join(", ", Columns.Select(c => $"[{c.ColumnName}]").ToArray());
            string referenceColumnList = string.Join(", ", _referencing.Columns.Select(c => $"[{c.ColumnName}]").ToArray());
            string onDelete = _cascadeDelete ? " ON DELETE CASCADE" : "";
            string onUpdate = _cascadeUpdate ? " ON UPDATE CASCADE" : "";

            string[] sql =
            {
                $@"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]
    ADD CONSTRAINT [FK_{TableName}_{indexTail}] FOREIGN KEY ({columnList})
        REFERENCES [{_referencing.DatabaseName}].[{_referencing.SchemaName}].[{_referencing.TableName}] ({referenceColumnList}){onDelete}{onUpdate}"
            };

            return sql;
        }

        public override string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected, IGraphVisitor graph)
        {
            string indexTail = string.Join("_", Columns.Select(c => $"{c.ColumnName}").ToArray());

            string[] sql =
            {
                $"ALTER TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]\r\n    DROP CONSTRAINT [FK_{TableName}_{indexTail}]"
            };

            return sql;
        }

        internal override string GenerateDefinitionSql()
        {
            string indexTail = string.Join("_", Columns.Select(c => $"{c.ColumnName}").ToArray());
            string columnList = string.Join(", ", Columns.Select(c => $"[{c.ColumnName}]").ToArray());
            string referenceColumnList = string.Join(", ", _referencing.Columns.Select(c => $"[{c.ColumnName}]").ToArray());
            string onDelete = _cascadeDelete ? " ON DELETE CASCADE" : "";
            string onUpdate = _cascadeUpdate ? " ON UPDATE CASCADE" : "";

            return $@"
    CONSTRAINT [FK_{TableName}_{indexTail}] FOREIGN KEY ({columnList})
        REFERENCES [{_referencing.DatabaseName}].[{_referencing.SchemaName}].[{_referencing.TableName}] ({referenceColumnList}){onDelete}{onUpdate}";
        }

        protected override BigInteger ComputeSha256Hash()
        {
            return nameof(CreateForeignKeyMigration).Sha256Hash().Concatenate(
                _parent.Sha256Hash,
                _referencing.Sha256Hash,
                new BigInteger((_cascadeDelete ? 2 : 0) + (_cascadeUpdate ? 1 : 0)));
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(CreateForeignKeyMigration),
                new Dictionary<string, string>
                {
                    ["CascadeDelete"] = _cascadeDelete ? "true" : "false",
                    ["CascaseUpdate"] = _cascadeUpdate ? "true" : "false"
                },
                Sha256Hash,
                new Dictionary<string, IEnumerable<BigInteger>>
                {
                    ["Prerequisites"] = Prerequisites.Select(x => x.Sha256Hash),
                    ["Parent"] = new[] { _parent.Sha256Hash },
                    ["Referencing"] = new[] { _referencing.Sha256Hash }
                });
        }

        public static CreateForeignKeyMigration FromMemento(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            return new CreateForeignKeyMigration(
                (CreateIndexMigration)migrationsByHashCode[memento.Prerequisites["Parent"].Single()],
                (CreatePrimaryKeyMigration)migrationsByHashCode[memento.Prerequisites["Referencing"].Single()],
                memento.Attributes["CascadeDelete"] == "true",
                memento.Attributes["CascaseUpdate"] == "true",
                memento.Prerequisites["Prerequisites"].Select(x => migrationsByHashCode[x]).ToImmutableList());
        }
    }
}