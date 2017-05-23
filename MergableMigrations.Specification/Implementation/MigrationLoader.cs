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
                case nameof(UseSchemaMigration):
                    return new UseSchemaMigration(
                        memento.Attributes["DatabaseName"],
                        memento.Attributes["SchemaName"]);
                case nameof(CreateTableMigration):
                    return new CreateTableMigration(
                        (UseSchemaMigration)migrationsByHashCode[memento.Prerequisites["Parent"].Single()],
                        memento.Attributes["TableName"]);
                case nameof(CreateColumnMigration):
                    return new CreateColumnMigration(
                        (CreateTableMigration)migrationsByHashCode[memento.Prerequisites["Parent"].Single()],
                        memento.Attributes["ColumnName"],
                        memento.Attributes["TypeDescriptor"]);
                default:
                    throw new ArgumentException($"Unknown type {memento.Type}");
            }
        }
    }
}