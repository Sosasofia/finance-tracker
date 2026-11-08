# FinanceTracker overview

FinanceTracker is a full-stack personal finance application. It helps
authenticated users record income and expenses, review spending patterns, and
export their transaction data.

## Features

- Google OpenID Connect and email/password authentication
- Stateless JWT authentication for API requests
- Transaction creation, viewing, editing, soft deletion, and restoration
- Categories, payment methods, installments, and reimbursements
- Dashboard totals and recent activity
- Analytics by transaction type, category, and timeframe
- CSV and Excel exports
- Receipt scanning through Azure AI Document Intelligence
- API health checks, rate limiting, structured error responses, and telemetry

## Architecture and layers

The repository is organized as a backend and frontend:

```text
backend/
  FinanceTracker.Domain          Core entities, value objects, and rules
  FinanceTracker.Application     Use cases, commands, queries, and validation
  FinanceTracker.Infrastructure  EF Core, repositories, auth, and integrations
  FinanceTracker.Server          ASP.NET Core API and composition root
frontend/                        Angular single-page web application
tests/                           Backend tests
```

The backend follows Clean Architecture and feature-based use cases. Each layer
has a focused responsibility so that business rules can be tested independently
from the database, external services, and web interface.

### Domain

The Domain layer contains the core financial concepts and rules. It defines
entities such as users, transactions, categories, installments, and
reimbursements, as well as value objects such as money. This layer does not
depend on the database, ASP.NET Core, or external services. It is responsible
for protecting business rules, such as valid transaction amounts and safe
state changes.

### Application

The Application layer describes what the system can do. It organizes use cases
for authentication, transactions, categories, payments, exports, and receipt
processing. It coordinates validation, calls the domain model, and uses
interfaces for persistence and integrations. It does not contain database
implementation details or HTTP-specific behavior.

### Infrastructure

The Infrastructure layer connects the application to outside systems. It
implements Entity Framework Core persistence and repositories for SQL Server,
authentication services, password hashing, Google token validation, file
exports, Azure AI receipt scanning, and observability integrations.

### Server

The Server layer is the ASP.NET Core API and application entry point. It
receives HTTP requests through controllers, identifies the signed-in user,
configures dependency injection and middleware, and returns API responses. It
also configures authentication, rate limiting, health checks, Swagger, CORS,
error handling, and the SPA proxy used during local development. Business rules
belong in the Domain and Application layers rather than in controllers.

The main request flow is:

```text
Angular app -> API controller -> application request -> validation
-> domain rules/repository -> SQL Server
```

See [Architecture.md](1-Architecture.md) for the full technical design and
[API_Documentation.md](2-API_Documentation.md) for available endpoints.

For an index of all project documentation, see [Documentation](README.md).

## Technology stack

| Area | Technology |
| --- | --- |
| Backend | .NET 8, ASP.NET Core Web API, C# |
| Application patterns | Clean Architecture, CQRS, feature-based use cases |
| Validation | FluentValidation |
| Data access | Entity Framework Core 8 and SQL Server |
| Frontend | Angular 22, TypeScript, RxJS |
| UI | Angular Material |
| State | NgRx Signal Store |
| Authentication | Google OpenID Connect, JWT, BCrypt |
| Receipt scanning | Azure AI Document Intelligence |
| Observability | OpenTelemetry, Serilog, Aspire Dashboard |
| Hosting | Azure App Service and Vercel |

## Local development

### Prerequisites

- Git
- .NET 8 SDK
- Node.js and npm
- A SQL Server instance
- Google OAuth credentials if Google sign-in is needed
- Azure Document Intelligence credentials if receipt scanning is needed

### Configure the API

The API project is `backend\FinanceTracker.Server`. Use .NET user secrets or
environment variables for secrets; do not commit credentials.

```powershell
cd backend\FinanceTracker.Server
dotnet user-secrets set "Jwt:Key" "a-long-random-secret"
dotnet user-secrets set "Jwt:Issuer" "finance-tracker-api"
dotnet user-secrets set "Jwt:ExpiryMinutes" "90"
dotnet user-secrets set "ConnectionStrings:FinanceDB" "your-sql-server-connection-string"
dotnet user-secrets set "Authentication:Google:ClientId" "your-google-client-id"
```

The frontend reads its public settings from `frontend\.env`. Start with
[`frontend\.env.example`](../frontend/.env.example) and set
`NG_APP_API_URL` and, when needed, `NG_APP_GOOGLE_CLIENT_ID`.

### Start the application

From the repository root, restore the backend and install the frontend
dependencies:

```powershell
dotnet restore
cd frontend
npm install
cd ..
```

Start the backend with the `https` launch profile:

```powershell
dotnet run --project backend\FinanceTracker.Server --launch-profile https
```

The backend launch settings enable the ASP.NET Core SPA proxy, which starts the
Angular development server automatically. This means the backend and frontend
run with one command rather than requiring two terminals. The application is
available at `https://localhost:57861/`, and the API is available at
`https://localhost:7036/`.

The API applies pending EF Core migrations when running in Development. Make
sure the configured SQL Server is reachable before starting the application.

### Optional telemetry dashboard

The compose file starts only the local Aspire Dashboard; it does not provide a
database server.

```powershell
docker compose up -d
```

Open `http://localhost:18888` to view telemetry. Stop it with:

```powershell
docker compose down
```

### Tests and checks

Backend:

```powershell
dotnet test tests\FinanceTracker.Test\FinanceTracker.Test.csproj
dotnet build backend\FinanceTracker.Server\FinanceTracker.Server.csproj --configuration Release
```

Frontend:

```powershell
cd frontend
npm run lint
npm test -- --watch=false --browsers=ChromeHeadless
npm run build
```

## Authentication flow

FinanceTracker supports Google sign-in and email/password authentication:

1. The user chooses Google sign-in or enters their email and password in the
   Angular application.
2. The frontend sends the login information to the API.
3. For Google sign-in, the API validates the Google ID token with Google's
   public signing keys. For a local account, it verifies the stored password
   hash.
4. After successful validation, the API creates and returns a signed JWT
   containing the authenticated user's identity.
5. The frontend stores the token and sends it in the `Authorization: Bearer`
   header for protected API requests.
6. The API reads the user identity from the token and scopes transaction data
   to that user before passing the request to the Application layer.
7. When the token expires or is invalid, the API rejects the request and the
   user must authenticate again.

## Deployment

The API is configured for deployment to Azure App Service and the Angular
frontend is configured for deployment to Vercel. GitHub Actions provides the
repository automation for build, test, and deployment workflows. Deployment
settings and secrets must be configured in the hosting platforms rather than
committed to the repository.

## Future roadmap

- Dark mode
- Budget and savings alerts
- More budgeting and planning tools

## Related documentation

- [Local setup guide](3-Setup.md)
- [System architecture](1-Architecture.md)
- [Product overview](PRODUCT.md)
- [API documentation](2-API_Documentation.md)
- [Documentation index](README.md)
