# 04-validation: Validate solution build and backend tests

Run restore, the Release backend build, repository formatting verification, and the `FinanceTracker.Test` xUnit suite using the commands defined in `.github/copilot-instructions.md`. Investigate and fix any upgrade-related failures, and record the final validation results and changed files without running Git commands or touching the protected pending directories.

## Research Findings

- The repository instructions require `dotnet restore`, a Release build of `src/FinanceTracker.Server/FinanceTracker.Server.csproj`; the current solution layout places the server at `backend/FinanceTracker.Server/FinanceTracker.Server.csproj`, so validation will use the actual solution path while also validating the server project directly.
- Formatting is checked with `dotnet format .\FinanceTracker.sln --verify-no-changes --verbosity diagnostic`.
- Backend tests are in `tests/FinanceTracker.Test/FinanceTracker.Test.csproj` and use xUnit; the full project test command is required after the TFM/package migration.
- Task 2 restore and Task 3 full Release solution build already passed with zero errors and zero warnings. This task must independently confirm restore, build, formatting, and tests after the complete change set.
- Protected existing `docs/todo/` and `services/` changes must remain untouched, and no Git commands are permitted.
