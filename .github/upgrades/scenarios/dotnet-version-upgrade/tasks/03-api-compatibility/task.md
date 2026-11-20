# 03-api-compatibility: Fix .NET 10 compilation and behavior changes

Resolve the assessment's binary and source API compatibility findings across Application, Infrastructure, and Server, including JWT, configuration, time-span, and related framework/package API usage. Preserve domain invariants, MediatR feature boundaries, repository scoping, authentication behavior, JSON conventions, and existing exception handling. Address all compiler errors and warnings introduced or exposed by the target framework and dependency updates without modifying `docs/` or `services/`.

## Research Findings

- Application assessment findings include the `ActivitySource.StartActivity` behavioral-change signal in `backend/FinanceTracker.Application/Common/Behaviors/OpenTelemetryBehavior.cs`; the package and TFM findings were handled in Task 2.
- Infrastructure has 34 API findings, primarily IdentityModel/JWT compatibility signals in `Services/AuthenticationExtensions.cs` and `Services/AuthInfrastructureService.cs`, plus Azure Blob URI/SAS behavior in `Services/AzureBlobStorageService.cs` and BinaryData usage in `Services/AzureReceiptScannerService.cs`.
- Server has eight source-compatibility and six behavioral API findings in `Configuration/DependencyInjection.cs`, `Configuration/ObservabilityExtensions.cs`, and `Middleware/GlobalExceptionHandler.cs`; these include rate-limit `TimeSpan` values, OTLP `Uri` construction, and exception-handler APIs.
- The assessment lists JWT types/constructors and `BinaryData.FromStreamAsync` as affected API surfaces. These are package/API compatibility signals to validate against the restored .NET 10 dependency graph rather than replacing established authentication or receipt-scanning behavior speculatively.
- No stubs exist. The implementation must preserve JWT cookie extraction (`auth_token`), issuer/signing-key validation, EF Core persistence, rate-limit windows, OpenTelemetry configuration, and the existing global exception-to-problem-details mapping.
