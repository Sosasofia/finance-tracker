# Local Development Setup

This guide provides step-by-step instructions for setting up and running the FinanceTracker application on your local machine.

## Prerequisites

Before you begin, ensure you have the following tools installed:
* **Git**: For cloning the repository.
* **.NET 8 SDK**: To build and run the backend API.
* **Node.js and npm**: To build and run the Angular frontend.
* **Docker**: To run the local MySQL database.
* **An IDE/Editor**: Such as Visual Studio 2022.


## Installation and Configuration

### 1. Clone the repository

```bash
git clone https://github.com/sosasofia/finance-tracker.git
cd finance-tracker
```

### 2. Install dependencies

#### Backend (.NET API)
```bash
cd src/FinanceTracker.API
dotnet restore
```

#### Frontend (Angular)
```bash
cd financetracker.client
npm install
```

### 3. Set up environment variables & secrets

#### Frontend

Create a `.env` file in `financetracker.client` using the provided `.env.example` template:

- `financetracker.client/.env`

See [financetracker.client/.env.example](financetracker.client/.env.example) for required variables.

#### Backend

For the backend, use user secrets or environment variables. In development, configure secrets using the [Secret Manager tool](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```bash
cd src/FinanceTracker.API

# JWT Configuration
dotnet user-secrets set "Jwt:Key" "your-jwt-secret-key-here"
dotnet user-secrets set "Jwt:Issuer" "your-app-name-or-domain"

# Google OAuth Configuration
dotnet user-secrets set "Authentication:Google:ClientId" "your-google-oauth-client-id"

# Database Connection (if different from appsettings.json)
dotnet user-secrets set "ConnectionStrings:FinanceDB" "your-database-connection-string"
```

**Required user secrets:**
- `Jwt:Key` - Secret key for JWT token signing (use a strong, random string)
- `Jwt:Issuer` - JWT token issuer (typically your app name or domain)
- `Authentication:Google:ClientId` - Google OAuth client ID from [Google Cloud Console](https://console.cloud.google.com/)
- `ConnectionStrings:FinanceDB` - Database connection string (optional if using default from appsettings.json)

### 4. Database Setup (Local Development)

For local development, you can use the provided Docker Compose setup to run MySQL and phpMyAdmin:

```bash
# Start MySQL and phpMyAdmin containers
docker compose -f docker-compose.dev.yml up -d

# Stop containers when done
docker compose -f docker-compose.dev.yml down

# Stop containers and delete volume
docker compose -f docker-compose.dev.yml down -v
```

This will start:
- **MySQL 9.3** on `localhost:3306`
- **phpMyAdmin** on `localhost:8080` for database management

**Local database connection string:**

```bash
"ConnectionStrings:FinanceDB": "Server=localhost;Port=3306;Database=finance;User=<your-username>;Password=<your-password>;SslMode=Preferred;"
```
> ⚠️ Replace `<your-username>` and `<your-password>` with your own credentials. Do not commit sensitive information to source control.


### 5. Running locally

You can run both the backend and frontend together using Visual Studio. When you start the backend project, Visual Studio automatically launches the frontend via its proxy setup.

> No need to run frontend separately—just start the backend project in Visual Studio and both will be available.

Visit [https://localhost:57861/](https://localhost:57861/) to access the application.

---

## Testing

### Backend
```bash
cd FinanceTracker.Test
dotnet test
```

### Frontend
```bash
cd financetracker.client
ng test
```