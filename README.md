# Mergable Migrations

Partially ordered database migrations for .NET.

Current status: **Proof of Concept**

You can't yet use this in production.

## What do migrations look like?

Create a schema (or use `dbo`):

```csharp
public void AddMigrations(DatabaseSpecification db)
{
    var dbo = db.UseSchema("dbo");

    var mathematicianKey = DefineMathematician(dbo);

    DefineContribution(dbo, mathematicianKey);
}
```

Create a table:

```csharp
private static PrimaryKeySpecification DefineMathematician(SchemaSpecification schema)
{
    var table = schema.CreateTable("Mathematician");

    var mathematicianId = table.CreateIntColumn("MathematicianId");
    var pk = table.CreatePrimaryKey(mathematicianId);
    var name = table.CreateStringColumn("Name", 100);
    var birthYear = table.CreateIntColumn("BirthYear");
    var deathYear = table.CreateIntColumn("DeathYear", nullable: true);

    return pk;
}
```

Create a foreign key:

```csharp
private static void DefineContribution(SchemaSpecification schema, PrimaryKeySpecification mathematicianKey)
{
    var table = schema.CreateTable("Contribution");

    var mathematicianId = table.CreateIntColumn("MathematicianId");

    var indexMathematicianId = table.CreateIndex(mathematicianId);
    var fkMathematician = indexMathematicianId.CreateForeignKey(mathematicianKey);
}
```

Organize these migration steps to keep them nice and neat. Put them in functions. Group them in classes. The organization is up to you. They don't need to be put in sequential order.

If you make a mistake, do not delete a migration step! Instead, create a new migration. Rename a table or column:

```csharp
private static void DefineContribution(SchemaSpecification schema, PrimaryKeySpecification mathematicianKey)
{
    // ...

    var paper = table.RenameTable("Paper");
}

private static PrimaryKeySpecification DefineMathematician(SchemaSpecification schema)
{
    // ...

    var yearOfBirth = birthYear.RenameColumn("YearOfBirth");

    // ...
}
```

Or drop a table or column:

```csharp
private static PrimaryKeySpecification DefineMathematician(SchemaSpecification schema)
{
    // ...

    deathDate.DropColumn();

    // ...
}
```

As long as you always add migrations, database deployment will be successful. To any environment. In any order. But if you delete one, then the tool will halt.

## Why partially ordered?

The reason that database migrations are so finicky is that they have to be applied in a specific order. This makes it difficult for members of a development team to work on different migrations at the same time. Usually, they have to resolve collisions by backing out and reapplying their changes. This ensures that changes can be serialized: applied in a specific linear order.

But if you examine the dependencies between migrations, you will find that most of the time parallel migrations can be applied in either order. That's because at their core, these changes are actually partially ordered. It's just that our tools don't know about that partial order.

If a set of migrations are totally ordered, then for any pair of migrations, I can tell which one has to happen before the other. This is usually done by comparing their sequence numbers. If you can make this comparison, then your set is totally ordered: it is a sequence.

If a set is partially ordered, however, then for any pair of migrations, I might be able to tell which comes first, or I might not. This seems less useful, but in fact it gives you more options. If a set of migrations is partially ordered, then that means that different developers can be working on different migrations in parallel. They each think that their migrations came first. By the time they integrate their migrations, they can be applied in either order.

## Yet another migration tool?

What about Entity Framework Migrations? FluentMigrator? RoundhousE? Django?

All of these tools assume a totally ordered sequence of migrations. Total ordering makes merging hard. Just Google merging in any of these projects and you will see how difficult it is.

Mergable Migrations is the first tool that defines a partial order of migrations. I don't expect it to be the last, but until then, enjoy the perks that only partial order can give you.

* Simple merging on multi-developer teams
* Elimination of unnecessary intermediate steps
* Aggregation of migrations for optimal change scripts
* Consolidation of migrations by table
* Organization of code the way you want

You are in complete control of your database migrations, but you don't have to manage their dependencies and order anymore.