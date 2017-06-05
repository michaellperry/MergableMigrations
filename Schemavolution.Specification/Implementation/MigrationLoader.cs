using Schemavolution.Specification.Migrations;
using System;
using System.Collections.Immutable;
using System.Numerics;

namespace Schemavolution.Specification.Implementation
{
    static class MigrationLoader
    {
        public static Migration Load(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            switch (memento.Type)
            {
                case nameof(UseSchemaMigration):
                    return UseSchemaMigration.FromMemento(memento, migrationsByHashCode);
                case nameof(CreateTableMigration):
                    return CreateTableMigration.FromMemento(memento, migrationsByHashCode);
                case nameof(CreateColumnMigration):
                    return CreateColumnMigration.FromMemento(memento, migrationsByHashCode);
                case nameof(CreatePrimaryKeyMigration):
                    return CreatePrimaryKeyMigration.FromMemento(memento, migrationsByHashCode);
                case nameof(CreateUniqueIndexMigration):
                    return CreateUniqueIndexMigration.FromMemento(memento, migrationsByHashCode);
                case nameof(CreateIndexMigration):
                    return CreateIndexMigration.FromMemento(memento, migrationsByHashCode);
                case nameof(CreateForeignKeyMigration):
                    return CreateForeignKeyMigration.FromMemento(memento, migrationsByHashCode);
                case nameof(CustomSqlMigration):
                    return CustomSqlMigration.FromMemento(memento, migrationsByHashCode);
                default:
                    throw new ArgumentException($"Unknown type {memento.Type}");
            }
        }
    }
}