﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Schemavolution.Specification.Implementation;

namespace Schemavolution.Specification.Migrations
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

        public CreateTableMigration(UseSchemaMigration parent, string tableName, ImmutableList<Migration> prerequisites) :
            base(prerequisites)
        {
            _parent = parent;
            _tableName = tableName;
        }

        public override IEnumerable<Migration> AllPrerequisites => Prerequisites
            .Concat(new[] { _parent });

        internal void AddDefinition(TableDefinitionMigration childMigration)
        {
            _definitions = _definitions.Add(childMigration);
        }

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected, IGraphVisitor graph)
        {
            string createTable;
            string head = $"CREATE TABLE [{DatabaseName}].[{SchemaName}].[{TableName}]";
            var optimizableMigrations = _definitions
                .SelectMany(m => graph.PullPrerequisitesForward(m, this, CanOptimize))
                .ToImmutableList();
            if (optimizableMigrations.Any())
            {
                var definitions = optimizableMigrations
                    .OfType<TableDefinitionMigration>()
                    .Select(d => d.GenerateDefinitionSql());
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
            migrationsAffected.AppendAll(optimizableMigrations);

            return sql;
        }

        private bool CanOptimize(Migration migration)
        {
            if (migration is TableDefinitionMigration definition)
            {
                return definition.CreateTableMigration == this;
            }
            else if (migration is CustomSqlMigration)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected, IGraphVisitor graph)
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
                    ["Prerequisites"] = Prerequisites.Select(x => x.Sha256Hash),
                    ["Parent"] = new[] { _parent.Sha256Hash }
                });
        }

        public static CreateTableMigration FromMemento(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            return new CreateTableMigration(
                (UseSchemaMigration)migrationsByHashCode[memento.Prerequisites["Parent"].Single()],
                memento.Attributes["TableName"],
                memento.Prerequisites["Prerequisites"].Select(p => migrationsByHashCode[p]).ToImmutableList());
        }
    }
}