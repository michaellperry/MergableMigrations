﻿using System;
using Schemavolution.DDD;
using Schemavolution.Specification;

namespace Mathematicians
{
    // Again, something that you wouldn't actually do, but this supports
    // the tests about rolling back a migration.
    public class MigrationsV3 : IGenome
    {
        public void AddGenes(DatabaseSpecification db)
        {
            var dbo = db.UseSchema("dbo");

            var mathematician = DefineMathematician(dbo);
            var field = DefineField(dbo);

            DefineContribution(dbo, mathematician, field);
        }

        private static AggregateRoot DefineMathematician(SchemaSpecification schema)
        {
            var table = schema.CreateTable("Mathematician");

            var mathematicianId = table.CreateIdentityColumn("MathematicianId");
            var pk = table.CreatePrimaryKey(mathematicianId);
            var name = table.CreateStringColumn("Name", 100);
            var birthYear = table.CreateIntColumn("BirthYear");
            //var deathYear = table.CreateIntColumn("DeathYear", nullable: true);

            return new AggregateRoot(pk);
        }

        private AggregateRoot DefineField(SchemaSpecification schema)
        {
            var table = schema.CreateTable("Field");

            var id = table.CreateIdentityColumn("FieldId");
            var pk = table.CreatePrimaryKey(id);
            var name = table.CreateStringColumn("Name", 20);

            return new AggregateRoot(pk);
        }

        private static void DefineContribution(SchemaSpecification schema, AggregateRoot mathematician, AggregateRoot field)
        {
            var table = schema.CreateTable("Contribution");

            var contributionId = table.CreateIdentityColumn("ContributionId");
            var pk = table.CreatePrimaryKey(contributionId);
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
