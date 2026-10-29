# System Architecture & Technical Specifications

This document outlines the architectural design, layer boundaries, data flow, component hierarchies, and deployment configurations for the **FinanceTracker** application.

---

## 1. System Overview

**FinanceTracker** is a personal finance management web application that allows authenticated users to record, categorize, and track incomes and expenses, manage multi-installment credit purchases and reimbursements, scan receipts via AI, and visualize financial habits with monthly statistics.

### Core Tech Stack

* **Backend:** .NET 8 (C#), ASP.NET Core Web API, Entity Framework Core 8, MediatR, FluentValidation, BCrypt.Net.
* **Frontend:** Angular 19, TypeScript, Angular Material, NgRx Signal Store (`@ngrx/signals`), RxJS.
* **Database & ORM:** SQL Server / Azure SQL (configurable via EF Core; local development stack includes MySQL 9.3 with phpMyAdmin).
* **Security & Identity:** Google OpenID Connect (OIDC) client verification + Stateless JWT bearer tokens, ASP.NET Core Rate Limiting policies.
* **Integrations:** Azure AI Document Intelligence (`prebuilt-receipt` model), MiniExcel (CSV & XLSX exports).

---

## 2. Backend Architecture

The backend follows **Clean Architecture** (Ports & Adapters) principles with **CQRS** orchestrated by MediatR.

```
                      ┌────────────────────────────────────────┐
                      │    FinanceTracker.Server (API Root)    │
                      └──────────────────┬─────────────────────┘
                                         │ depends on
                                         ▼
                      ┌────────────────────────────────────────┐
                      │       FinanceTracker.Application       │
                      │        (CQRS, Use Cases, DTOs)         │
                      └──────────┬──────────────────▲──────────┘
             depends on (domain) │                  │ implements ports
                                 ▼                  │
┌────────────────────────────────────────┐  ┌───────┴────────────────────────────┐
│         FinanceTracker.Domain          │  │   FinanceTracker.Infrastructure    │
│  (Entities, ValueObjects, Exceptions)  │◄─┤   (EF Core, Repositories, Azure)   │
└────────────────────────────────────────┘  └────────────────────────────────────┘
```

### Layer Boundaries & Responsibilities

### 2.1 Domain Layer (`src/FinanceTracker.Domain`)
Completely decoupled from databases, UI, and external frameworks.
* **Entities:**
  * `Transaction`: Aggregate root managing amounts, categories, payment methods, installments, reimbursements, audit metadata, and soft delete state.
  * `User`: Application user model supporting both Google OAuth and local password credentials.
  * `Category`: Income/expense classification.
  * `PaymentMethod`: Methods of transaction payment (Cash, Credit Card, Bank Transfer, etc.).
  * `Installment`: Sub-entity for credit card installment breakdowns.
  * `Reimbursement`: Sub-entity tracking partial or full expense reimbursements.
* **Value Objects:**
  * `Money`: Encapsulates an amount and currency code with validation and arithmetic consistency.
  * `ExtractedReceiptData`: Immutable representation of OCR-extracted data from receipts.
* **Domain-Driven Design (DDD) Patterns:**
  * Entities protect their invariants via private setters and factory methods (e.g., `Transaction.Create(...)`).
  * Domain behavior methods enforce rules before mutating state: `UpdateDetails`, `GenerateInstallments`, `AddReimbursement`, `ChangeAmount`, `ChangeDate`, `AssignCategory`, `SoftDelete`, and `Restore`.
  * Aggregates encapsulate child collections (`IReadOnlyCollection<Installment>`).
  * Custom exceptions derive from `DomainException`.

### 2.2 Application Layer (`src/FinanceTracker.Application`)
Orchestrates business use cases organized as feature vertical slices under `Features/<Feature>/{Commands,Queries}`:
* **Features:**
  * `Auth`: `LoginCommand`, `RegisterCommand`, `GoogleLoginCommand`.
  * `Transactions`:
    * Commands: `CreateTransactionCommand`, `UpdateTransactionCommand`, `DeleteTransactionCommand`, `RestoreTransactionCommand`.
    * Queries: `GetTransactionsQuery`, `GetTransactionByIdQuery`, `ExportTransactionsQuery`.
  * `Categories`: Commands and queries for category retrieval and maintenance.
  * `PaymentMethods`: Queries and commands for payment method management.
  * `Users`: User profile retrieval.
* **Pipeline Behaviors:**
  * `ValidationBehavior<TRequest, TResponse>`: Automatically executes FluentValidation validators before requests reach handlers, failing fast with a `ValidationException`.
* **Services:**
  * `ReceiptMappingService`: Converts raw OCR receipt data into pre-filled transaction input DTOs.

### 2.3 Infrastructure Layer (`src/FinanceTracker.Infrastructure`)
Supplies concrete implementations of interfaces defined in Domain and Application.
* **Data Access & EF Core:**
  * `ApplicationDbContext`: Defines table mappings (`transaction`, `user`, `category`, `payment_method`, `installment`, `reimbursement`).
  * **Value Object Mapping:** `Money` is configured as an owned entity (`builder.OwnsOne(t => t.Money)`).
  * **Soft Delete Global Query Filter:** `builder.HasQueryFilter(t => !t.IsDeleted)` excludes soft-deleted items automatically.
  * **Restore Operations:** Use `.IgnoreQueryFilters()` in `TransactionRepository.GetTransactionByIdAndUserIncludingDeletedAsync`.
  * **Repositories:** Concrete implementations (`TransactionRepository`, `CategoryRepository`, `PaymentMethodRepository`, `UserRepository`).
* **External Adapters:**
  * `AuthInfrastructureService`: Validates Google ID tokens via `GoogleJsonWebSignature`, generates HMAC-SHA256 JWTs, and handles BCrypt password hashing.
  * `AzureReceiptScannerService`: Communicates with Azure AI Document Intelligence.
  * `MiniExcelFileGenerator`: Generates streaming CSV and XLSX byte streams.

### 2.4 Presentation / API Layer (`src/FinanceTracker.Server`)
HTTP composition root and endpoint definitions.
* **Controllers:**
  * `AuthController` (`/api/auth`): Login, registration, and Google OAuth callback.
  * `TransactionController` (`/api/transactions`): Authenticated CRUD, restore, and CSV/Excel downloads.
  * `CategoryController` (`/api/categories`): Category listing and management.
  * `PaymentMethodController` (`/api/payment-methods`): Payment method retrieval.
  * `ReceiptsController` (`/api/receipts`): Multipart file upload for OCR scanning.
* **Security & Current User:**
  * Injects `ICurrentUserService` to safely retrieve the authenticated user's ID from JWT claims (`NameIdentifier`), ensuring all domain calls are tenant-isolated.
* **Cross-Cutting Concerns:**
  * `GlobalExceptionHandler`: Implements `IExceptionHandler` and maps domain, validation, authorization, not-found, and external gateway errors to RFC 7807 `ProblemDetails`.
  * **Rate Limiting:** Granular policies (`auth-limit` for brute-force prevention, `fixed` per-user/IP limits, `download-limit` for exports, and `receipt-scanner-policy` for OCR quotas).

---

## 3. Frontend Architecture

The client in `financetracker.client` is an Angular 19 single-page application built using standalone components.

### 3.1 Component Hierarchy & Routing Strategy
Configured in `src/app/app.routes.ts`:

```
AppComponent
 ├── /login ──> LoginComponent (GoogleSignInComponent + Standard Form)
 └── / (guarded by AuthGuard) ──> MainLayoutComponent
      ├── /dashboard  ──> DashboardComponent
      ├── /income     ──> IncomeComponent
      ├── /expense    ──> ExpenseComponent
      └── /analytics  ──> AnalyticsComponent
```

* **Guards:** `AuthGuard` checks token validity via `AuthService.isLoggedIn()` before granting access to authenticated routes, redirecting unauthenticated requests to `/login`.
* **Layout Structure:** `MainLayoutComponent` acts as the persistent shell providing responsive navigation, theme toggling, and user profile display around `<router-outlet>`.

### 3.2 State Management (`@ngrx/signals`)
* **TransactionStore (`src/app/features/transactions/state/transaction.store.ts`):**
  * Root-provided NgRx Signal Store.
  * **State Slice:**
    ```typescript
    interface TransactionState {
      transactions: Transaction[];
      isLoading: boolean;
      error: string | null;
    }
    ```
  * **Computed Signals:** `totalIncome` and `totalExpense` are derived reactively from current transaction lists.
  * **Methods:** `loadTransactions()`, `deleteTransaction(id)`, `addTransactionLocal(tx)`.
  * **Lifecycle Hooks:** Automatically initiates `loadTransactions()` on application bootstrap via `withHooks({ onInit })`.

### 3.3 HTTP Communication & Interceptors
* **Services:**
  * `AuthService`: Manages user credentials, JWT decode checks, login/logout operations, and auth state observables.
  * `TransactionService`: Executes HTTP requests for transactions, categories, payment methods, and file blob downloads.
  * `ReceiptScannerService`: Uploads receipt image/PDF binaries to the backend OCR endpoint.
* **Interceptors:**
  * `TokenInterceptor`: Attaches `Authorization: bearer <token>` headers to outgoing HTTP calls using tokens stored in `localStorage`.
  * `ErrorInterceptor` & `GlobalErrorInterceptor`: Intercept API failures and display toast/snack-bar alerts.
  * `LoadingSpinnerInterceptor`: Coordinates loading state across active network requests.

---

## 4. Data Flow & Integrations

### 4.1 Authentication Flow
```
[User] ──> [Google Sign-In Button] ──> [Google Identity Services]
                                                    │
                                                    ▼ (idToken)
[Angular Client] ── POST /api/auth/google-login ──> [ASP.NET Core API]
                                                    │
                                                    ▼
                                          [Validate via Google APIs]
                                                    │
                                                    ▼
                                      [Lookup or Create User in DB]
                                                    │
                                                    ▼
                                            [Generate JWT]
                                                    │
                                                    ▼
[Angular Client] ◄────── Returns JWT ───────────────┘
  │
  ├──> Stores in localStorage ('auth_token')
  └──> TokenInterceptor attaches token to subsequent requests
```

### 4.2 Receipt Scanning Flow
```
[User Uploads Receipt (JPG, PNG, PDF)]
         │
         ▼
[POST /api/receipts/scan]
         │
         ▼ (Rate limiter: receipt-scanner-policy max 10/hr)
[AzureReceiptScannerService] ──> [Azure AI Document Intelligence]
                                              │
                                              ▼ (Raw JSON / Prebuilt Receipt)
[ReceiptMappingService] ◄─────────────────────┘
  │
  ├── Extracts merchant name, total, date, category hints
  └── Returns ExtractedReceiptData DTO to Angular frontend form
```

---

## 5. Infrastructure & Deployment

### 5.1 Containerization & Local Development
* **Backend Dockerfile:** Multi-stage build (`mcr.microsoft.com/dotnet/sdk:8.0` build stage -> `mcr.microsoft.com/dotnet/aspnet:8.0` runtime image).
* **Local Services (`docker-compose.dev.yaml`):** Provides a local MySQL 9.3 container and phpMyAdmin instance.
* **PaaS Deployments:** Configured for Railway deployment via `railway.json`.

### 5.2 CI/CD Pipelines (GitHub Actions)
* **Continuous Integration (`.github/workflows/ci.yml`):**
  * Triggers on pull requests and pushes to `main` and `dev`.
  * Validates .NET code formatting (`dotnet format --verify-no-changes`) and builds the backend in `Release` mode.
  * Installs Node 20 dependencies, executes ESLint (`npm run lint`), and executes headless Karma unit tests (`npm run test -- --watch=false --browsers=ChromeHeadless`).
* **Continuous Deployment (`.github/workflows/cd.yml`):**
  * Triggers on merges to `main`.
  * Publishes `FinanceTracker.Server` as an artifact.
  * Authenticates to Microsoft Azure using passwordless OpenID Connect (`azure/login@v2`).
  * Deploys directly to Azure App Service (`finance-tracker`, slot `Production`).
* **Client Hosting:** Deployed to Vercel with SPA URL rewrites handled via `vercel.json`.

### 5.3 Required Configuration & Environment Variables

| Environment / Variable | Location / Target | Purpose |
|---|---|---|
| `ConnectionStrings:FinanceDB` | Backend Config / Secrets | Database connection string (SQL Server / Azure SQL) |
| `Jwt:Key` | Backend Config / Secrets | Symmetric signing key for JWT generation |
| `Jwt:Issuer` | Backend Config / Secrets | JWT issuer and audience claim |
| `Jwt:ExpiryMinutes` | Backend Config / Secrets | Token expiration duration |
| `Authentication:Google:ClientId` | Backend Config / Secrets | Google client ID for backend token validation |
| `AzureDesign:DocumentIntelligence:Endpoint` | Backend Config / Secrets | Azure Document Intelligence service endpoint |
| `AzureDesign:DocumentIntelligence:ApiKey` | Backend Config / Secrets | Azure Document Intelligence API key |
| `FRONTEND_URL` | Backend Config / Environment | Allowed CORS origin |
| `NG_APP_API_URL` | Frontend `.env` | Base backend API endpoint URL |
| `NG_APP_GOOGLE_CLIENT_ID` | Frontend `.env` | Google OAuth Client ID for GIS button |

---

## 6. Directory Structure Reference

```
finance-tracker/
├── .github/
│   └── workflows/
│       ├── ci.yml                     # Continuous Integration workflow
│       └── cd.yml                     # Azure App Service deployment workflow
├── docs/
│   ├── API_Documentation.md          # REST API endpoint reference
│   ├── Architecture.md               # Architecture and design specifications
│   └── Setup.md                      # Local environment setup instructions
├── src/
│   ├── FinanceTracker.Domain/         # Core entities, value objects, domain interfaces
│   ├── FinanceTracker.Application/    # MediatR commands, queries, validators, use cases
│   ├── FinanceTracker.Infrastructure/ # EF Core DbContext, repositories, external services
│   └── FinanceTracker.Server/         # ASP.NET Core API, controllers, middleware, Program.cs
├── financetracker.client/             # Angular 19 client application
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/                  # Singleton services, auth guards, interceptors
│   │   │   ├── features/              # Feature modules (dashboard, income, expense, analytics)
│   │   │   ├── layout/                # MainLayoutComponent, shell components
│   │   │   └── shared/                # Shared UI components and models
│   │   └── environments/              # Frontend environment configs
│   ├── tools/set-env.js               # Injects .env variables into environment files
│   └── vercel.json                    # Vercel SPA routing rewrite config
├── Dockerfile                         # Production container definition for backend API
├── docker-compose.dev.yaml            # Local development compose stack
└── FinanceTracker.sln                 # .NET solution file
```

---

## 7. Architectural Improvements & Technical Debt (Ranked by Urgency)

### Tier 1: Critical Urgency (Security & Secrets Management)
1. **Decouple Secrets from Configuration:**
   * **Issue:** Fallback JWT keys and connection strings have historically been stored in configuration files like `appsettings.development.json`.
   * **Remediation:** Enforce the use of [.NET Secret Manager (`dotnet user-secrets`)](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for all local development, integrate Azure Key Vault for production cloud environments, and establish pre-commit scanners (e.g., `git-secrets`) to block committed keys.

### Tier 2: High Urgency (Correctness & Quality Gates)
2. **Resolve Database Provider Parity (SQL Server vs. MySQL):**
   * **Issue:** `docker-compose.dev.yaml` starts MySQL 9.3, but the backend is hard-wired to SQL Server (`options.UseSqlServer(...)`), causing local container mismatch.
   * **Remediation:** Replace MySQL with `mcr.microsoft.com/mssql/server:2022-latest` (or Azure SQL Edge for ARM/Apple Silicon) in `docker-compose.dev.yaml`, or build a multi-provider database factory in `InfrastructureDependencyInjection`.
3. **Incorporate Backend Tests in CI Pipeline:**
   * **Issue:** `.github/workflows/ci.yml` runs formatting and build checks, but omits `dotnet test`, allowing backend regression failures to bypass automated pull request validation.
   * **Remediation:** Add `dotnet test FinanceTracker.Test/FinanceTracker.Test.csproj --no-build --configuration Release` to the `backend-validation` job in `ci.yml`.

### Tier 3: Medium Urgency (Auth Lifecycle & Resilience)
4. **Harden Token Storage Against XSS:**
   * **Issue:** The JWT is stored in browser `localStorage`, making it susceptible to exfiltration via Cross-Site Scripting (XSS).
   * **Remediation:** Transition authentication tokens to `HttpOnly`, `SameSite=Strict`, `Secure` cookies with anti-CSRF protection, or enforce strict Content Security Policy (CSP) headers.
5. **Implement Token Refresh & Silent Renewal Flow:**
   * **Issue:** Fixed expiration JWTs (90 mins) cause abrupt session drops and 401 errors once expired without any renewal mechanism.
   * **Remediation:** Introduce a rotatable Refresh Token entity in the domain model and attach an Angular HTTP interceptor to seamlessly renew expired tokens.
6. **External Integration Circuit Breakers & Resilience:**
   * **Issue:** Synchronous HTTP calls to Azure Document Intelligence and Google token verification lack fault tolerance or backoff retries.
   * **Remediation:** Implement resilience pipelines using `Microsoft.Extensions.Http.Resilience` / Polly around external API clients.

### Tier 4: Low Urgency (Observability & Operational Scalability)
7. **Automated Migration Execution in CD:**
   * **Issue:** Runtime migrations are disabled in `Program.cs`, requiring manual database schema updates in production.
   * **Remediation:** Generate an EF Core Migration Bundle (`dotnet ef migrations bundle`) during CI and execute it automatically in the CD pipeline prior to deployment.
8. **Structured Logging and Distributed Tracing:**
   * **Issue:** Basic console logging is used without correlation IDs across Angular and .NET.
   * **Remediation:** Integrate Serilog/OpenTelemetry with an `X-Correlation-ID` header forwarded across frontend-to-backend requests.

