# FinanceTracker

FinanceTracker is a web app for keeping track of income, expenses, and spending
habits in one place. Users can sign in, record transactions, view monthly
statistics, and export their data.

## What it includes

- Google or email/password sign-in
- Income and expense tracking
- Categories, filters, search, and date ranges
- Monthly balance and spending analytics
- CSV and Excel exports
- Receipt scanning with Azure AI Document Intelligence

## Technology

- **Web app:** Angular and TypeScript
- **API:** .NET 8 and ASP.NET Core
- **Database:** SQL Server with Entity Framework Core
- **Authentication:** Google sign-in and JWT
- **UI:** Angular Material
- **Hosting:** Azure App Service (API) and Vercel (web app)

## Run it locally

You need the .NET 8 SDK, Node.js, npm, and access to a SQL Server database.

1. Configure the required API secrets and connection string. See the
   [setup guide](docs/3-Setup.md).
2. Install the frontend dependencies once:

   ```powershell
   cd frontend
   npm install
   cd ..
   ```

3. Start the backend and frontend together through the configured SPA proxy:

   ```powershell
   dotnet run --project backend\FinanceTracker.Server --launch-profile https
   ```

The backend launch profile starts the Angular development server through the
ASP.NET Core SPA proxy. The application is available at
`https://localhost:57861/`.

## Future features

- Dark mode
- Budget and savings alerts
- More budgeting and planning tools

For technical details, architecture, authentication, deployment, and the
complete development notes, see the [project overview](docs/0-Overview.md).

## License

This project is licensed under the MIT License.
