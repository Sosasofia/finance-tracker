# Local Development Setup

This guide explains how to set up and run FinanceTracker locally.

## Prerequisites

- Git
- .NET 8 SDK
- Node.js and npm
- A SQL Server instance
- An IDE/editor, such as Visual Studio 2022

Google OAuth credentials are needed for Google sign-in. Azure AI Document
Intelligence credentials are needed for receipt scanning.

## Installation

### 1. Clone the repository

```powershell
git clone https://github.com/sosasofia/finance-tracker.git
cd finance-tracker
```

### 2. Install dependencies

Restore the backend:

```powershell
dotnet restore
```

Install the frontend dependencies:

```powershell
cd frontend
npm install
cd ..
```

### 3. Configure the frontend

Copy `frontend\.env.example` to `frontend\.env` and set:

- `NG_APP_API_URL`: the local API URL
- `NG_APP_GOOGLE_CLIENT_ID`: the Google OAuth client ID, if Google sign-in is
  enabled

The frontend environment file is generated during its start/build commands.
Do not commit credentials or tokens.

### 4. Configure the API

The API project is `backend\FinanceTracker.Server`. Use .NET user secrets or
environment variables instead of committing secrets:

```powershell
cd backend\FinanceTracker.Server
dotnet user-secrets set "Jwt:Key" "a-long-random-secret"
dotnet user-secrets set "Jwt:Issuer" "finance-tracker-api"
dotnet user-secrets set "Jwt:ExpiryMinutes" "90"
dotnet user-secrets set "ConnectionStrings:FinanceDB" "your-sql-server-connection-string"
dotnet user-secrets set "Authentication:Google:ClientId" "your-google-client-id"
cd ..\..
```

For receipt scanning, also configure:

```powershell
dotnet user-secrets set "AzureDesign:DocumentIntelligence:Endpoint" "your-endpoint"
```

The API uses `DefaultAzureCredential`: in Azure it selects the app's managed
identity, while local development can use an Azure CLI or Visual Studio login
(for example, `az login`). Grant whichever identity is used the `Cognitive
Services User` role on the Document Intelligence resource. In Azure, grant the
app's managed identity the `Cognitive Services User` role on
the Document Intelligence resource. The endpoint must be the resource's
custom-subdomain endpoint (for example,
`https://my-document-intelligence.cognitiveservices.azure.com/`); regional
endpoints do not support Microsoft Entra authentication. Set it as an
application setting named `AzureDesign__DocumentIntelligence__Endpoint`. For a
user-assigned identity, also set
`AzureDesign__DocumentIntelligence__ManagedIdentityClientId` to its client ID;
omit it for a system-assigned identity.

For local receipt image storage, configure either a development Storage
connection string or an account URL. Keep these values in user secrets or
environment variables:

```powershell
dotnet user-secrets set "BlobStorage:ConnectionString" "your-development-storage-connection-string"
dotnet user-secrets set "BlobStorage:ContainerName" "receipts"
```

In Azure, prefer `BlobStorage:AccountUrl` together with the application's
Managed Identity instead of a connection string. Grant the identity the
least-privileged Blob data role required for the private `receipts` container.
The container must remain private. Uploads are limited to 5 MB and are marked
as orphaned until a later transaction-association workflow is implemented;
configure a separate retention process for orphan cleanup.

The API issues short-lived, read-only user delegation SAS URLs through
`GET /api/receipts/{receiptId}/url`. When using Managed Identity, grant the
backend identity `Storage Blob Data Contributor` at the container scope for
blob operations and `Storage Blob Delegator` at the storage-account scope to
request a user delegation key. For local development with
`BlobStorage:ConnectionString`, the API uses
the storage account key only on the server to generate the SAS; it is never
sent to the frontend. Set `BlobStorage:SasLifetimeMinutes` to a value from 1
to 60 (the default is 15).

If Angular downloads the SAS URL with `fetch` or `HttpClient`, configure Blob
Storage CORS for the deployed frontend origin and allow `GET` and `HEAD`.
Direct image navigation does not require the browser to expose the response
to JavaScript, but the container must still remain private.

The receipt library is available from the dashboard. Users can save JPG, JPEG,
PNG, and PDF files with a display name, open them through a short-lived SAS
URL, delete them, or process them to prefill the expense form. Processing
never creates an expense automatically.

## Running locally

From the repository root, restore the backend and install the frontend
dependencies once:

```powershell
dotnet restore
cd frontend
npm install
cd ..
```

Start both the API and frontend with the backend's configured `https` launch
profile:

```powershell
dotnet run --project backend\FinanceTracker.Server --launch-profile https
```

The launch profile enables the ASP.NET Core SPA proxy, so it starts the Angular
development server automatically. No second terminal is required.

Open `https://localhost:57861/` for the application. The API runs at
`https://localhost:7036/`.

The API applies pending EF Core migrations when running in Development, including
the receipt table migration. Make sure the configured SQL Server is reachable
before starting or restarting the API after a migration is added. If the API
was already running when the migration was created, stop and start it again so
the migration runs before uploading a receipt.

## Optional telemetry dashboard

The repository's Docker Compose file starts only the Aspire Dashboard; it does
not start SQL Server or any other database.

```powershell
docker compose up -d
```

Open `http://localhost:18888` to view telemetry. Stop the dashboard with:

```powershell
docker compose down
```

## Testing

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
