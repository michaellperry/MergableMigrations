﻿using MergableMigrations.DDD;
using MergableMigrations.Specification;

namespace Mathematicians
{
    public class Migrations : IMigrations
    {
        public void AddMigrations(ModelSpecification model)
        {
            var db = model.CreateDatabase("Mathematicians");

            var dbo = db.UseSchema("dbo");

            var mathematician = DefineMathematician(dbo);

            DefineContribution(dbo, mathematician);
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

        private static void DefineContribution(SchemaSpecification schema, AggregateRoot mathematician)
        {
            var table = schema.CreateTable("Contribution");

            var contributionId = table.CreateIntColumn("ContributionId");
            var mathematicianId = table.CreateIntColumn("MathematicianId");
            var description = table.CreateStringColumn("Description", 500);

            var indexMathematicianId = table.CreateIndex(mathematicianId);
            var fkMathematician = indexMathematicianId.CreateForeignKey(mathematician.PrimaryKey);
        }
    }
}