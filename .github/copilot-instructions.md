# Copilot Instructions for FinanceTracker

## Build, test, and lint

The repository contains a .NET 8 API and an Angular 19 client. Run commands from the repository root unless a working directory is shown.

### Backend

```powershell
dotnet restore
dotnet build src\FinanceTracker.Server\FinanceTracker.Server.csproj --configuration Release
dotnet format .\FinanceTracker.sln --verify-no-changes --verbosity diagnostic
dotnet test FinanceTracker.Test\FinanceTracker.Test.csproj
```

Run one xUnit test with a fully qualified name filter:

```powershell
dotnet test FinanceTracker.Test\FinanceTracker.Test.csproj --filter "FullyQualifiedName~TransactionControllerTests.CreateTransaction_ReturnsCreatedAtAction_WhenCommandIsValid"
```

The test project is `FinanceTracker.Test` and currently contains controller tests using xUnit and Moq.

### Frontend

Install dependencies and run the same checks used by CI:

```powershell
cd financetracker.client
npm ci
npm run lint
npm run test -- --watch=false --browsers=ChromeHeadless
npm run build
```

Other useful client scripts are `npm start`, `npm run format`, and `npm run format:check`. Run a single Angular/Karma spec by passing its path to Angular CLI:

```powershell
npm test -- --watch=false --browsers=ChromeHeadless --include=src/app/core/auth/auth.service.spec.ts
```

The CI workflow validates backend formatting and Release build, then runs frontend lint and headless tests. The deployment workflow publishes only the backend project.

### Local services and configuration

`docker-compose.dev.yaml` starts MySQL and phpMyAdmin for local database work. The current backend DI configuration uses `UseSqlServer`, and the development settings point at LocalDB, so confirm the active connection string/provider before relying on the Compose MySQL service.

Use `financetracker.client\.env` based on `.env.example` for client settings. Configure backend secrets through .NET user secrets or environment variables, especially `Jwt:Key`, `Jwt:Issuer`, `Jwt:ExpiryMinutes`, `Authentication:Google:ClientId`, `ConnectionStrings:FinanceDB`, and Azure Document Intelligence settings. Do not put credentials or tokens in committed configuration.

## Architecture

- `src\FinanceTracker.Domain` is the dependency-free core. It contains entities, value objects, enums, domain interfaces, and domain exceptions. Domain entities enforce invariants through methods rather than exposing public setters; `Money` is the transaction amount value object.
- `src\FinanceTracker.Application` implements use cases by feature. Commands and queries are MediatR requests, normally organized as `Features\<Feature>\Commands` or `Queries`, with a request record, handler, and validators/models as needed. Application code depends on domain abstractions and service interfaces, not infrastructure implementations.
- `src\FinanceTracker.Infrastructure` supplies EF Core persistence, repositories, migrations, authentication/JWT and Google integration, file generation, and Azure receipt scanning. Register implementations in `InfrastructureDependencyInjection`.
- `src\FinanceTracker.Server` is the HTTP composition root. Controllers authenticate the request, obtain the current user ID, enrich commands/queries, and delegate to MediatR; they should not contain business rules or direct database access. `Program.cs` wires CORS, authentication, rate limiting, exception handling, Swagger, and controllers.
- `financetracker.client\src\app` is the Angular presentation layer. Routes lazy-load feature components under the authenticated `MainLayoutComponent`; shared/core services call the API and cross-cutting HTTP behavior is registered through `CoreModule`.
- `TransactionStore` is a root-provided NgRx Signal Store for transaction state, loading/error state, and derived totals. Keep server operations in services/store methods and keep components focused on UI orchestration.
- The normal request path is: Angular component/store -> Angular service/HTTP interceptor -> authorized API controller -> current-user service + MediatR request -> validation pipeline -> application handler -> domain entity/repository -> EF Core database. `GlobalExceptionHandler` maps validation, domain, authorization, not-found, and external-service failures to problem-details responses.

## Repository-specific conventions

- Preserve the feature-by-use-case layout in Application. Add a command/query record, handler, and validator beside related use cases rather than adding a generic service method or putting business logic in a controller.
- Controllers use `ICurrentUserService.UserId()` and pass the authenticated user ID into application requests. Handlers and repositories must scope reads and writes by that user ID; do not trust a user ID supplied by the browser.
- MediatR validation is global through `ValidationBehavior<,>` and FluentValidation validators discovered from the Application assembly. Add request validation to the feature instead of duplicating checks in controllers.
- Map business failures to the existing exception types (`ValidationException`, domain exceptions, `NotFoundException`, `ForbiddenAccessException`, or `UnauthorizedAccessException`) so `GlobalExceptionHandler` preserves the API error shape.
- Transactions are soft-deleted: normal reads exclude deleted rows through EF query filters, while restore uses a repository method that calls `IgnoreQueryFilters()`. Preserve this distinction when changing transaction queries or delete/restore behavior.
- Repository methods should use EF Core async APIs and accept/pass through `CancellationToken`. Use `AsNoTracking()` for read-only projections where the existing repository pattern does so.
- Keep domain state changes inside entity methods such as `ChangeAmount`, `ChangeDate`, `AssignCategory`, `SoftDelete`, and `Restore`; do not bypass invariants by setting persistence properties directly.
- Backend JSON uses camelCase and string enum values. API routes are under `/api`, and protected resource controllers use `[Authorize]` plus the configured rate-limit policy.
- Angular API URLs come from `src\environments\environment*.ts`; use those values rather than hardcoding endpoints. The auth/token interceptor stores the JWT under `auth_token` and adds it to protected requests.
- Angular components and services follow the existing standalone/lazy-loaded style, while `CoreModule` remains the single registration point for singleton auth services and HTTP interceptors. Constructor injection is allowed by the ESLint configuration.
- Follow the existing Angular selector conventions: `app-` plus kebab-case for components and `app` plus camelCase for directives. Keep formatting compatible with the repository Prettier configuration.
- When changing API contracts, update the corresponding Angular model/service and `docs\API_Documentation.md`; when changing setup or required settings, update `docs\Setup.md`.
