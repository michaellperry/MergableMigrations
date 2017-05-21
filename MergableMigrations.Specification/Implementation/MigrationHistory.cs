using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;

namespace MergableMigrations.Specification.Implementation
{
    public class MigrationHistory
    {
        private readonly ImmutableList<Migration> _migrations;
        private readonly ImmutableHashSet<Migration> _migrationSet;

        public MigrationHistory()
        {
            _migrations = ImmutableList<Migration>.Empty;
            _migrationSet = ImmutableHashSet<Migration>.Empty;
        }

        private MigrationHistory(ImmutableList<Migration> migrations, ImmutableHashSet<Migration> migrationSet)
        {
            _migrations = migrations;
            _migrationSet = migrationSet;
        }

        public MigrationHistory Add(Migration migration)
        {
            return new MigrationHistory(
                _migrations.Add(migration),
                _migrationSet.Add(migration));
        }

        public MigrationHistory AddAll(IEnumerable<Migration> migrations)
        {
            return new MigrationHistory(
                _migrations.AddRange(migrations),
                _migrationSet.Union(migrations));
        }

        public MigrationDelta Subtract(MigrationHistory migrationHistory)
        {
            return new MigrationDelta(_migrations.RemoveAll(x =>
                migrationHistory.Contains(x)));
        }

        public bool Contains(Migration migration)
        {
            return _migrationSet.Contains(migration);
        }

        public IEnumerable<MigrationMemento> GetMementos()
        {
            return _migrations.Select(m => m.GetMemento());
        }

        public static MigrationHistory LoadMementos(MigrationMemento[] mementos)
        {
            var migrations = ImmutableList<Migration>.Empty;
            var migrationsByHashCode = ImmutableDictionary<BigInteger, Migration>.Empty;

            foreach (var memento in mementos)
            {
                var migration = MigrationLoader.Load(memento, migrationsByHashCode);
                migrations = migrations.Add(migration);
                migrationsByHashCode = migrationsByHashCode
                    .Add(migration.Sha256Hash, migration);

            }

            var migrationSet = migrations.ToImmutableHashSet();

            return new MigrationHistory(migrations, migrationSet);
        }
    }
}
