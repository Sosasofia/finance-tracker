# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade the FinanceTracker solution from .NET 8 to .NET 10 while preserving the existing backend architecture, Angular integration, and repository conventions.
**Scope**: Six solution projects: five .NET projects and the Angular `frontend.esproj`; all .NET projects are SDK-style and currently target modern .NET.

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: Six projects, all on .NET 8, with a clear dependency structure and a primarily mechanical framework upgrade.

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | All-at-Once | The solution is small, SDK-style, modern .NET, and the assessment reports low project difficulty. |
| Package Management | Central Package Management (CPM) | Multiple projects have divergent package versions, so central management can improve consistency during the upgrade. |
| Unsupported Packages | Resolve Inline | The assessment identifies one incompatible package, which is within the inline resolution scope. |
| Unsupported API Handling | Fix Inline | The assessment identifies binary and source incompatibilities, and inline fixes are appropriate for this modern-to-modern upgrade. |

## Tasks

### 01-prerequisites: Verify .NET 10 upgrade prerequisites

Verify the installed .NET 10 SDK, global.json constraints, solution project inventory, and existing package-management layout before changing project files. Confirm that all projects are SDK-style, identify the single incompatible package and framework/API findings from the assessment, and preserve the existing pending `docs/` and `services/` changes.

**Done when**: The required .NET 10 SDK and solution prerequisites are confirmed, affected projects and package/API risks are documented in the task record, and no protected pending files are changed.

### 02-framework-and-packages: Upgrade project frameworks and dependencies

Update the five .NET projects from `net8.0` to `net10.0` in one atomic pass, preserving the Angular `frontend.esproj` integration and existing project references. Apply compatible .NET 10 package versions identified by the assessment, introduce Central Package Management only where it preserves project-specific metadata such as private assets, and remove or replace framework-included/deprecated references as appropriate. Restore the solution and resolve package compatibility issues without changing the existing application architecture.

**Done when**: All applicable .NET projects target `net10.0`, package references restore without conflicts, the incompatible package is resolved inline, and the Angular project remains correctly referenced by the server.

### 03-api-compatibility: Fix .NET 10 compilation and behavior changes

Resolve the assessment's binary and source API compatibility findings across Application, Infrastructure, and Server, including JWT, configuration, time-span, and related framework/package API usage. Preserve domain invariants, MediatR feature boundaries, repository scoping, authentication behavior, JSON conventions, and existing exception handling. Address all compiler errors and warnings introduced or exposed by the target framework and dependency updates without modifying `docs/` or `services/`.

**Done when**: The upgraded solution compiles with zero errors and zero warnings, all inline API changes are implemented without stubs, and the existing architecture and conventions remain intact.

### 04-validation: Validate solution build and backend tests

Run restore, the Release backend build, repository formatting verification, and the `FinanceTracker.Test` xUnit suite using the commands defined in `.github/copilot-instructions.md`. Investigate and fix any upgrade-related failures, and record the final validation results and changed files without running Git commands or touching the protected pending directories.

**Done when**: The backend Release build, relevant formatting check, and backend test suite pass; any intentionally unavailable validation is documented with its exact reason; and the task record lists all changed files.
