# Progress Details

## Files changed

- `Directory.Packages.props` — added Central Package Management for all direct package references.
- `backend/FinanceTracker.Domain/FinanceTracker.Domain.csproj` — retargeted to `net10.0`.
- `backend/FinanceTracker.Application/FinanceTracker.Application.csproj` — retargeted to `net10.0` and removed per-project package versions.
- `backend/FinanceTracker.Infrastructure/FinanceTracker.Infrastructure.csproj` — retargeted to `net10.0`, removed per-project versions, and preserved the Azure Identity alias.
- `backend/FinanceTracker.Server/FinanceTracker.Server.csproj` — retargeted to `net10.0`, centralized package versions, and preserved EF Design/Tools asset metadata and Angular project reference behavior.
- `tests/FinanceTracker.Test/FinanceTracker.Test.csproj` — retargeted to `net10.0`, centralized package versions, removed framework-included `System.Net.Http` and `System.Text.RegularExpressions`, and updated xUnit to 2.9.3.

## Changes applied

- Updated all five .NET project target frameworks from `net8.0` to `net10.0`.
- Centralized direct package versions and upgraded assessment-recommended .NET 10 packages to 10.0.15.
- Removed deprecated/framework-included direct test references and retained required test package metadata.
- Preserved the Server reference to `frontend/frontend.esproj` with `ReferenceOutputAssembly=false`.
- No `docs/` or `services/` files were modified. No Git commands were run.

## Validation

- `dotnet restore .\FinanceTracker.sln`: passed with zero errors and zero warnings.
- Project XML verification: all five .NET projects report `net10.0`; no direct `Version=` attributes remain in the project files.
- Full compilation and tests are deferred to the API compatibility and final validation tasks.
