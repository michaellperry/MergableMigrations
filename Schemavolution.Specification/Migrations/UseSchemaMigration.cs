using System;
using System.Collections.Generic;
using System.Numerics;
using Schemavolution.Specification.Implementation;
using System.Linq;
using System.Collections.Immutable;

namespace Schemavolution.Specification.Migrations
{
    class UseSchemaMigration : Migration
    {
        private readonly string _databaseName;
        private readonly string _schemaName;

        public string DatabaseName => _databaseName;
        public string SchemaName => _schemaName;

        public UseSchemaMigration(string databaseName, string schemaName, ImmutableList<Migration> prerequisites) :
            base(prerequisites)
        {
            _databaseName = databaseName;
            _schemaName = schemaName;
        }

        public override IEnumerable<Migration> AllPrerequisites => Prerequisites;

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected, IGraphVisitor graph)
        {
            string[] sql = { };
            return sql;
        }

        public override string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected, IGraphVisitor graph)
        {
            throw new NotImplementedException();
        }

        protected override BigInteger ComputeSha256Hash()
        {
            return nameof(UseSchemaMigration).Sha256Hash().Concatenate(
                _schemaName.Sha256Hash());
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(UseSchemaMigration),
                new Dictionary<string, string>
                {
                    [nameof(DatabaseName)] = DatabaseName,
                    [nameof(SchemaName)] = SchemaName
                },
                Sha256Hash,
                new Dictionary<string, IEnumerable<BigInteger>>
                {
                    ["Prerequisites"] = Prerequisites.Select(x => x.Sha256Hash)
                });
        }

        public static UseSchemaMigration FromMemento(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            return new UseSchemaMigration(
                memento.Attributes["DatabaseName"],
                memento.Attributes["SchemaName"],
                memento.Prerequisites["Prerequisites"].Select(p => migrationsByHashCode[p]).ToImmutableList());
        }
    }
}