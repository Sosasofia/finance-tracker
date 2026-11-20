# .NET Version Upgrade

## Preferences
- **Flow Mode**: Guided
- **Target Framework**: net10.0
- **Scope**: All projects in FinanceTracker.sln
- **Repository Changes**: Do not modify existing `docs/` or `services/` changes.
- **Git Commands**: Do not run Git commands during this scenario.

## Source Control
- **Source Branch**: chore/upgrade-dotnet-10
- **Working Branch**: chore/upgrade-dotnet-10 (current branch)
- **Pending Changes**: Leave untouched; no commit, stash, or undo.
- **Commit Strategy**: After Each Task (when applicable; no Git commands requested)
- **Branch Sync**: Disabled

## Upgrade Options

### Strategy
- Upgrade Strategy: All-at-Once

### Project Structure
- Package Management: Central Package Management (CPM)

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package)
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-at-Once
**Rationale**: Six SDK-style projects are already on modern .NET, with a clear dependency structure and a primarily mechanical net8.0-to-net10.0 upgrade.

### Execution Constraints
- Upgrade all .NET projects in one atomic operation; do not introduce dependency-tier sequencing.
- Verify the installed .NET 10 SDK and global.json compatibility before editing project files.
- Create central package management only if it can preserve existing package behavior and project-specific metadata.
- Resolve the one incompatible package and framework/API compatibility issues inline rather than leaving stubs or deferred fixes.
- Run restore and full solution build after the upgrade, then run backend tests; treat warnings as errors for task completion.
- Do not modify existing `docs/` or `services/` changes and do not run Git commands.
