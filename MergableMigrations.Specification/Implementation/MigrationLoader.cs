using System;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;

namespace MergableMigrations.Specification.Implementation
{
    static class MigrationLoader
    {
        public static Migration Load(MigrationMemento memento, IImmutableDictionary<BigInteger, Migration> migrationsByHashCode)
        {
            switch (memento.Type)
            {
                case nameof(CreateDatabaseMigration):
                    return new CreateDatabaseMigration(
                        memento.Attributes["DatabaseName"]);
                case nameof(UseSchemaMigration):
                    return new UseSchemaMigration(
                        (CreateDatabaseMigration)migrationsByHashCode[memento.Prerequisites.First()],
                        memento.Attributes["SchemaName"]);
                case nameof(CreateTableMigration):
                    return new CreateTableMigration(
                        (UseSchemaMigration)migrationsByHashCode[memento.Prerequisites.First()],
                        memento.Attributes["TableName"]);
                case nameof(CreateColumnMigration):
                    return new CreateColumnMigration(
                        (CreateTableMigration)migrationsByHashCode[memento.Prerequisites.First()],
                        memento.Attributes["ColumnName"],
                        memento.Attributes["TypeDescriptor"]);
                default:
                    throw new ArgumentException($"Unknown type {memento.Type}");
            }
        }
    }
}