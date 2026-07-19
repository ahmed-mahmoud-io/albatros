# Contributing to Albatros

Thank you for contributing to Albatros. This document describes the coding standards, branching and PR process, and how to run and test the project.

## Guidelines

- Follow the rules in `.editorconfig` for formatting, indentation and naming.
- Keep methods small and focused. Prefer composition over inheritance.
- Use meaningful names and XML documentation where public APIs are exposed.
- All new features must include unit tests when reasonable.

## Standards

- Target framework: .NET 6+ (project-specific). Use C# language features consistent with the codebase.
- Dependency injection must be used for services and DbContext.
- Use EF Core migrations for database changes.
- Store secrets and connection strings in `appsettings.Development.json` or secure stores.

## Branching

- `main` is the production branch. Create feature branches named `feature/<short-description>`.
- Open a Pull Request when ready. Include a summary and testing steps.

## Pull Request Checklist

- Code builds locally.
- No compiler warnings or fix them if appropriate.
- Tests pass.
- `.editorconfig` rules followed.
- Update database migrations if schema changed.

## Local setup

1. Ensure SQL Server is installed and `DefaultConnection` in `appsettings.json` points to a reachable database.
2. Restore NuGet packages: `dotnet restore`.
3. Apply migrations: `dotnet ef database update`.
4. Run the app: `dotnet run` or use Visual Studio 2022.

## Contact

If you need help, open an issue or contact the repository maintainers.

-- End of CONTRIBUTING.md
