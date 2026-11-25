/*
  Adopt an EXISTING ClaimStatements database (built by the former EF6 migrations) into EF Core migrations.

  Run once per environment, AFTER the database team has compared the database's schema, constraint names and
  indexes with the InitialCreate migration (Generator\Context\Migrations). It creates and changes no tables: it
  only records InitialCreate as already applied, so the migration bundle starts from later migrations.

  Never run this against a new or empty database; the migration bundle creates those from scratch.
*/
IF OBJECT_ID(N'[SITI].[Claims]') IS NULL OR OBJECT_ID(N'[DAX].[AP]') IS NULL
    THROW 50000, 'Expected ClaimStatements tables not found. This is not an EF6-built ClaimStatements database; do not adopt it.', 1;

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20261119134049_InitialCreate')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261119134049_InitialCreate', N'10.0.13');
