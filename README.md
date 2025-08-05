# FinanceTracker


FinanceTracker is a personal finance tracking application built as a monorepo. It features a .NET 8 backend and an Angular frontend. Track income, expenses, and view monthly statistics. Authentication uses Google OAuth, with the backend issuing a JWT token for secure access.

---

## Technologies

- **Backend:** .NET 8, Entity Framework Core, MySQL
- **Frontend:** Angular 19, Angular Material
- **Authentication:** Google OAuth (OpenID Connect)
- **Hosting:** Backend on [Railway](https://railway.app), Frontend on [Vercel](https://vercel.com)

---

## Setup Instructions

### 1. Clone the repository

```bash
git clone https://github.com/sosasofia/finance-tracker.git
cd finance-tracker
```

### 2. Install dependencies

#### Backend (.NET API)
```bash
cd FinanceTracker.Server
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
cd FinanceTracker.Server

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

### 4. Running locally

You can run both the backend and frontend together using Visual Studio. When you start the backend project, Visual Studio automatically launches the frontend via its proxy setup.

> No need to run frontend separately—just start the backend project in Visual Studio and both will be available.

Visit [https://localhost:57861/](https://localhost:57861/) to access the application.

---


## Deployment & CI/CD

- **Backend:** [Railway](https://finance-tracker-server.up.railway.app)
- **Frontend:** [Vercel](https://finance-tracker-client.vercel.app)
- **CI/CD:** [GitHub Actions](https://github.com/sosasofia/finance-tracker/actions)  
    The current workflow (GitHub Actions pipeline) automatically deletes old and inactive GitHub deployments to keep the environment clean and efficient.

---

## Features

- ✅ Google OAuth login
- 🔜 Email/password login (coming soon)
- ✅ Income & expense CRUD operations
- 🔄 Monthly statistics and charts (in progress)
- 📤 Export to Excel (planned)
- 🌙 Dark mode (planned)
- 🔔 Budget/savings alerts (planned)


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

---

## 📄 License

This project is licensed under the MIT License.
