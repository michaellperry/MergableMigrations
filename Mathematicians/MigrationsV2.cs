using System;
using MergableMigrations.DDD;
using MergableMigrations.Specification;

namespace Mathematicians
{
    // You wouldn't actually do this, but to test upgrading to a new version,
    // I created a second migrations class.
    public class MigrationsV2 : IMigrations
    {
        public void AddMigrations(DatabaseSpecification db)
        {
            var dbo = db.UseSchema("dbo");

            var mathematician = DefineMathematician(dbo);
            var field = DefineField(dbo);

            DefineContribution(dbo, mathematician, field);
        }

        private static AggregateRoot DefineMathematician(SchemaSpecification schema)
        {
            var table = schema.CreateTable("Mathematician");

            var mathematicianId = table.CreateIntColumn("MathematicianId");
            var pk = table.CreatePrimaryKey(mathematicianId);
            var name = table.CreateStringColumn("Name", 100);
            var birthYear = table.CreateIntColumn("BirthYear");
            var deathYear = table.CreateIntColumn("DeathYear", nullable: true);

            return new AggregateRoot(pk);
        }

        private AggregateRoot DefineField(SchemaSpecification schema)
        {
            var table = schema.CreateTable("Field");

            var id = table.CreateIntColumn("FieldId");
            var pk = table.CreatePrimaryKey(id);
            var name = table.CreateStringColumn("Name", 20);

            return new AggregateRoot(pk);
        }

        private static void DefineContribution(SchemaSpecification schema, AggregateRoot mathematician, AggregateRoot field)
        {
            var table = schema.CreateTable("Contribution");

            var contributionId = table.CreateIntColumn("ContributionId");
            var mathematicianId = table.CreateIntColumn("MathematicianId");
            var description = table.CreateStringColumn("Description", 500);
            var fieldId = table.CreateIntColumn("FieldId");

            var indexMathematicianId = table.CreateIndex(mathematicianId);
            var fkMathematician = indexMathematicianId.CreateForeignKey(mathematician.PrimaryKey);

            var indexFieldId = table.CreateIndex(fieldId);
            var fkField = indexFieldId.CreateForeignKey(field.PrimaryKey);
        }
    }
}
