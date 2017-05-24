﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;

namespace MergableMigrations.Specification.Implementation
{
    class PrimaryKeyMigration : Migration
    {
        private readonly CreateTableMigration _parent;
        private readonly ImmutableList<CreateColumnMigration> _columns;

        public string DatabaseName => _parent.DatabaseName;
        public string SchemaName => _parent.SchemaName;
        public string TableName => _parent.TableName;

        public PrimaryKeyMigration(CreateTableMigration parent, IEnumerable<CreateColumnMigration> columns)
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

        protected override BigInteger ComputeSha256Hash()
        {
            return nameof(PrimaryKeyMigration).Sha256Hash().Concatenate(
                Enumerable.Repeat(_parent.Sha256Hash, 1)
                    .Concat(_columns.Select(c => c.Sha256Hash))
                    .ToArray());
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(PrimaryKeyMigration),
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

        public static PrimaryKeyMigration FromMemento(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            return new PrimaryKeyMigration(
                (CreateTableMigration)migrationsByHashCode[memento.Prerequisites["Parent"].Single()],
                memento.Prerequisites["Columns"].Select(p => migrationsByHashCode[p]).OfType<CreateColumnMigration>());
        }
    }
}