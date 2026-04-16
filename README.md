# FinanceTracker

FinanceTracker is a modern, full-stack web application designed to help users manage their personal finances, built with a focus on **Clean Architecture and robust security**. The application allows users to securely log in via their Google account, manage income and expenses, and visualize their spending habits with monthly statistics.

## Key Features

- **Secure Authentication**: Leverages **Google's OpenID Connect** for robust and secure user sign-in. The backend validates the Google token and issues a stateless **JWT** for authenticated API access.
- **Full Transaction Management**: Easily create, read, update, and delete income and expense records.
- **Data Visualization**: View insightful monthly statistics and charts to understand spending patterns.
- **Monorepo Structure**: A single repository hosts both the .NET backend and Angular frontend, simplifying development and dependency management.

## Architecture and Project Structure

This project is built using **Clean Architecture** principles to ensure separation of concerns, maintainability, and testability.

Initially developed with a traditional layered approach (Controllers, Services, Repositories), the project was intentionally refactored to follow Clean Architecture. This evolution was driven by the goal of isolating the core business logic from external dependencies like the database, authentication providers, and the user interface.

The core layers of the architecture are:

- **Domain**: Contains the core business logic and entities of the application. It has no dependencies on any other layer.
- **Application**: Orchestrates business logic using the CQRS (Command Query Responsibility Segregation) pattern with MediatR. This decouples the entry points from the business logic and allows for a clean, pipeline-based approach to cross-cutting concerns.
- **Infrastructure**: Provides concrete implementations for the interfaces defined in the Application layer. This includes database access with Entity Framework Core, integration with third-party services like Google OAuth, and other external concerns.
- **Presentation (API)**: The entry point to the system that handles HTTP requests and API configuration. For this project, it's a .NET 8 Web API that exposes endpoints for the Angular client. It depends on the Application layer.

This structure ensures that the application is flexible, scalable, and easy to test in isolation.

For detailed information on the available endpoints, see the **[API Documentation](docs/API_Documentation.md)**.

## Authentication Flow

Authentication is handled via a secure, token-based flow using a third-party provider:

1.  The user clicks "Login with Google" on the Angular frontend.
2.  The frontend redirects the user to Google's OAuth 2.0 consent screen.
3.  After successful authentication, Google redirects back to the frontend with an `IdToken`.
4.  The Angular client sends this `IdToken` to the .NET backend.
5.  The backend **validates the token's signature and payload** against Google's public keys.
6.  Upon successful validation, the backend generates a custom **JWT (JSON Web Token)** and returns it to the client.
7.  The Angular client stores this JWT and includes it in the `Authorization` header for all subsequent API requests to protected endpoints.

## Technologies

| Area           | Technology                                               |
| -------------- | -------------------------------------------------------- |
| **Backend**    | .NET 8, ASP.NET Core Web API, Entity Framework Core      |
| **Frontend**   | Angular 19, Angular Material, TypeScript                 |
| **Database**   | MySQL                                                    |
| **Auth**       | Google OAuth (OpenID Connect), JWT                       |
| **Hosting**    | Backend on **Azure App Service**, Frontend on **Vercel** |
| **Patterns**   | CQRS with MediatR                                        |
| **Validation** | FluentValidation (Automatic Pipeline)                    |
| **Security**   | ASP.NET Core Rate Limiting, CORS Policy                  |

## Getting Started

For detailed instructions on how to clone the repository, install dependencies, and run the application locally, please see the dedicated **[Setup Guide](docs/Setup.md)**.

## Deployment & CI/CD

- **Backend:** Azure App Service
- **Frontend:** [Vercel](https://finance-tracker-client.vercel.app)
- **CI/CD:** [GitHub Actions](https://github.com/sosasofia/finance-tracker/actions)
  - **Continuous Integration (CI)**: Runs automatically on pull requests and pushes to validate .NET code formatting, build the backend, and execute Angular linters and tests.
  - **Continuous Deployment (CD)**: Automatically builds and securely deploys the production-ready application to Azure App Service using passwordless authentication (OIDC) whenever code is merged into main.

## Future Roadmap

- [x] **Email/Password Login**: Implement a traditional identity system alongside OAuth.
- [x] **Export to Excel**: Allow users to export their transaction data.
- [ ] **Dark Mode**: Add a theme toggle for user preference.
- [ ] **Budget & Savings Alerts**: Implement notifications for budget limits and savings goals.

## License

This project is licensed under the MIT License.
