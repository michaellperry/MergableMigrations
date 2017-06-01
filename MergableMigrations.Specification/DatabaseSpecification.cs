using System;
using MergableMigrations.Specification.Implementation;
using MergableMigrations.Specification.Migrations;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace MergableMigrations.Specification
{
    public class DatabaseSpecification : Specification
    {
        private readonly string _databaseName;

        public MigrationHistory MigrationHistory => MigrationHistoryBuilder.MigrationHistory;
        internal override IEnumerable<Migration> Migrations => Enumerable.Empty<Migration>();

        public DatabaseSpecification(string databaseName) :
            base(new MigrationHistoryBuilder())
        {
            _databaseName = databaseName;
        }

        private DatabaseSpecification(string databaseName, MigrationHistoryBuilder migrationHistoryBuilder, ImmutableList<Migration> prerequisites) :
            base(migrationHistoryBuilder, prerequisites)
        {
            _databaseName = databaseName;
        }

        public DatabaseSpecification After(params Specification[] specifications)
        {
            return new DatabaseSpecification(_databaseName, MigrationHistoryBuilder,
                Prerequisites.AddRange(specifications.SelectMany(x => x.Migrations)));
        }

        public SchemaSpecification UseSchema(string schemaName)
        {
            var migration = new UseSchemaMigration(_databaseName, schemaName, Prerequisites);
            MigrationHistoryBuilder.Append(migration);
            migration.AddToParent();
            return new SchemaSpecification(migration, MigrationHistoryBuilder);
        }

        public CustomSqlSpecification Execute(string up, string down = null)
        {
            throw new NotImplementedException();
        }
    }
}