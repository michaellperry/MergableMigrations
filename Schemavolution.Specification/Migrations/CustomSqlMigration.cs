using Schemavolution.Specification.Implementation;
using System.Collections.Immutable;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace Schemavolution.Specification.Migrations
{
    class CustomSqlMigration : Migration
    {
        private readonly string _databaseName;
        private readonly string _up;
        private readonly string _down;

        public string DatabaseName => _databaseName;
        public string Up => _up;
        public string Down => _down;

        public CustomSqlMigration(string databaseName, string up, string down, ImmutableList<Migration> prerequisites) :
            base(prerequisites)
        {
            _databaseName = databaseName;
            _up = up;
            _down = down;
        }

        public override IEnumerable<Migration> AllPrerequisites => Prerequisites;

        public override string[] GenerateSql(MigrationHistoryBuilder migrationsAffected, IGraphVisitor graph)
        {
            return new string[] { $"USE {DatabaseName}", _up };
        }

        public override string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected, IGraphVisitor graph)
        {
            return
                _down != null ? new string[] { $"USE {DatabaseName}", _down } :
                new string[] { };
        }

        protected override BigInteger ComputeSha256Hash()
        {
            return nameof(CustomSqlMigration).Sha256Hash().Concatenate(
                _up.Sha256Hash(),
                _down.Sha256Hash());
        }

        internal override MigrationMemento GetMemento()
        {
            return new MigrationMemento(
                nameof(CustomSqlMigration),
                new Dictionary<string, string>
                {
                    [nameof(DatabaseName)] = DatabaseName,
                    [nameof(Up)] = Up,
                    [nameof(Down)] = Down
                },
                Sha256Hash,
                new Dictionary<string, IEnumerable<BigInteger>>
                {
                    ["Prerequisites"] = Prerequisites.Select(x => x.Sha256Hash)
                });
        }

        public static CustomSqlMigration FromMemento(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            return new CustomSqlMigration(
                memento.Attributes["DatabaseName"],
                memento.Attributes["Up"],
                memento.Attributes["Down"],
                memento.Prerequisites["Prerequisites"].Select(p => migrationsByHashCode[p]).ToImmutableList());
        }
    }
}