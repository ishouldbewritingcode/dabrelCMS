Run an EF Core migration for dabrelCMS. The database is SQLite at `App_Data/dabrelCMS.db`.

The migration name to use: $ARGUMENTS

Steps:
1. If no migration name was provided in $ARGUMENTS, ask the user for one before proceeding. Migration names should be PascalCase and describe the schema change (e.g. `AddUserRole`, `CreateBlockTable`).
2. Run `dotnet ef migrations add <MigrationName>` and check for errors. If it fails, report the error and stop — do not proceed to the update step.
3. Show the user what files were generated in the `Migrations/` folder.
4. Run `dotnet ef database update` and confirm success.
5. Report that the migration was applied to `App_Data/dabrelCMS.db`.

Important: never run `dotnet ef database drop` or delete migration files without explicit user confirmation.
