# Mergable Migrations

Partially ordered database migrations for .NET.

Current status: **ideation**

You can't yet use this in production.

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