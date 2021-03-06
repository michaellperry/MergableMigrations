﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;

namespace Schemavolution.Specification.Implementation
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

        public bool Empty =>
            !_migrations.Any();

        public bool Contains(Migration migration) =>
            _migrationSet.Contains(migration);

        public IEnumerable<MigrationMemento> GetMementos() =>
            _migrations.Select(m => m.GetMemento());

        public static MigrationHistory LoadMementos(IEnumerable<MigrationMemento> mementos)
        {
            var migrations = ImmutableList<Migration>.Empty;
            var migrationsByHashCode = ImmutableDictionary<BigInteger, Migration>.Empty;

            foreach (var memento in mementos)
            {
                var migration = MigrationLoader.Load(memento, migrationsByHashCode);
                if (migration.Sha256Hash != memento.HashCode)
                    throw new ArgumentException("Hash code does not match");
                migrations = migrations.Add(migration);
                migration.AddToParent();
                migrationsByHashCode = migrationsByHashCode
                    .Add(migration.Sha256Hash, migration);

            }

            var migrationSet = migrations.ToImmutableHashSet();

            return new MigrationHistory(migrations, migrationSet);
        }
    }
}
