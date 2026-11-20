# 01-prerequisites: Verify .NET 10 upgrade prerequisites

Verify the installed .NET 10 SDK, global.json constraints, solution project inventory, and existing package-management layout before changing project files. Confirm that all projects are SDK-style, identify the single incompatible package and framework/API findings from the assessment, and preserve the existing pending `docs/` and `services/` changes.

## Research Findings

- .NET 10 SDK validation succeeded; no `global.json` exists, so there is no SDK pin blocking the upgrade.
- The solution contains five SDK-style .NET projects currently targeting `net8.0`: Domain, Application, Infrastructure, Server, and FinanceTracker.Test. The sixth project is the existing Angular `frontend/frontend.esproj`, which uses the Visual Studio JavaScript SDK and remains outside the .NET TFM update.
- No `Directory.Packages.props` was found and package versions are currently defined per project. The server dependency inventory confirms direct package references include ASP.NET Core JWT/SpaProxy, EF Core, health checks, OpenTelemetry, Serilog, and Swashbuckle. CPM will need to preserve metadata such as `PrivateAssets`, `IncludeAssets`, and the SpaProxy wildcard behavior.
- Assessment risks to carry into the implementation task: one incompatible package, 13 recommended package upgrades, 11 binary-incompatible APIs, 24 source-incompatible APIs, and 14 behavioral-change findings. Main affected projects are Application, Infrastructure, and Server; the test project has deprecated/framework-included package findings.
- No `// STUB:` markers were found under `backend/**/*.cs`; no prior deferred stubs require resolution.
- Existing pending `docs/todo/` and `services/` changes were not inspected or modified, and no Git command was run.

**Done when**: The required .NET 10 SDK and solution prerequisites are confirmed, affected projects and package/API risks are documented in the task record, and no protected pending files are changed.
