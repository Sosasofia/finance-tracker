# MediatR Removal Report

## Summary

The FinanceTracker backend no longer uses the MediatR NuGet package. The application now uses an internal request dispatcher implemented within `FinanceTracker.Application`.

This change preserves the existing feature-by-use-case organization, including commands, queries, handlers, validators, and cross-cutting behaviors, without relying on MediatR abstractions or runtime registration.

## Packages Removed

The following project files no longer reference MediatR:

- `backend/FinanceTracker.Application/FinanceTracker.Application.csproj`
- `backend/FinanceTracker.Server/FinanceTracker.Server.csproj`

The backend source also no longer references:

- `MediatR`
- `ISender`
- `IPipelineBehavior`
- MediatR request and handler interfaces

## New Application Dispatcher

The replacement implementation is located under `backend/FinanceTracker.Application/Common`.

### Request contracts

`Common/Interfaces/IDispatcher.cs` defines the application-owned contracts:

- `IRequest<TResponse>` identifies a request and its response type.
- `IRequestHandler<TRequest, TResponse>` handles a request.
- `IRequestDispatcher` dispatches requests to their handlers.
- `IRequestDecorator<TRequest, TResponse>` represents cross-cutting request processing.
- `Unit` represents commands that do not return a domain response.

### Dispatcher implementation

`Common/Dispatcher/RequestDispatcher.cs`:

1. Receives an application request.
2. Determines the concrete request type and response type.
3. Resolves the matching handler from dependency injection.
4. Resolves registered decorators for that request.
5. Builds the decorator chain around the handler.
6. Executes the request while passing through the cancellation token.

The dispatcher explicitly closes the generic dispatch method using the request runtime type and response type. This avoids dynamic generic type inference failures while preserving the simple controller API:

```csharp
await _dispatcher.Send(request, cancellationToken);
```

## Cross-Cutting Behaviors

The former MediatR pipeline behaviors are now application decorators:

- `ValidationBehavior<TRequest, TResponse>` runs all applicable FluentValidation validators, aggregates validation failures, and throws the existing application `ValidationException` when validation fails.
- `OpenTelemetryBehavior<TRequest, TResponse>` creates an application request activity, records success or failure status, and attaches exceptions to failed activities.

Both decorators are registered as open generic services in `ApplicationDependencyInjection` and are applied by the internal dispatcher.

## Dependency Injection

`ApplicationDependencyInjection.AddApplicationServices()` now:

- Registers `IRequestDispatcher` with `RequestDispatcher`.
- Registers validation and telemetry decorators.
- Scans the Application assembly for implementations of `IRequestHandler<,>`.
- Registers discovered handlers with scoped lifetime.
- Continues discovering FluentValidation validators from the Application assembly.

## Feature and API Changes

All existing application commands, queries, and handlers remain organized by feature. Their MediatR interfaces were replaced with the internal application contracts.

Commands that previously returned no value now use `Unit`, including account, category, payment method, and transaction deletion commands.

All API controllers now inject `IRequestDispatcher` instead of `ISender`. Controller responsibilities remain unchanged:

- Authenticate requests.
- Obtain the current user ID.
- Enrich commands and queries with trusted user context.
- Dispatch application requests.
- Return the appropriate HTTP response.

## Testing and Validation

The controller tests were updated to mock `IRequestDispatcher` instead of `ISender`.

Validation completed successfully:

- `dotnet restore FinanceTracker.sln`
- Solution build
- Release build of `FinanceTracker.Server`
- `dotnet format FinanceTracker.sln --verify-no-changes`
- FinanceTracker.Test test suite

All 6 existing tests passed.

## Result

The backend no longer depends on MediatR. Request dispatching, handler discovery, validation, telemetry, cancellation propagation, and controller behavior are now implemented using application-owned abstractions and the built-in .NET dependency injection container.
