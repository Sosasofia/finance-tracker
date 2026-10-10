# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

Primary users are individuals managing everyday income, expenses, and monthly spending. They use FinanceTracker to keep a current view of their finances and understand where their money is going.

## Product Purpose

FinanceTracker helps people securely record income and expenses, review recent activity, and understand spending habits through monthly statistics and category-based analytics. Success means a user can maintain an accurate personal finance record and make sense of it without needing a complex budgeting system.

## Positioning

FinanceTracker is a secure, simple view of spending habits that combines straightforward transaction management with useful monthly statistics. Its distinct mechanism is the connection between quick income/expense capture and immediately understandable analytics, rather than requiring users to configure an elaborate financial planning workflow.

## Operating Context

Users sign in through Google or email/password, then work inside an authenticated dashboard. They add, edit, filter, search, and review income and expense transactions, inspect analytics by transaction type and timeframe, and export their transaction data as CSV or Excel when needed. The product is a responsive web application used across desktop and mobile-sized screens.

## Capabilities and Constraints

- Authentication supports Google OpenID Connect and email/password sign-in.
- The client uses a stateless JWT issued by the backend for authenticated API access.
- Users can create, read, update, and delete income and expense records.
- The dashboard exposes total balance, monthly income, monthly expenses, recent activity, category filtering, name search, date filtering, and data export.
- Analytics supports income/expense views, this month, last month, and all-time timeframes, with category-based charts.
- The current product terminology includes Dashboard, Income, Expense, Analytics, transaction, category, and total balance.
- The product is a web application that must remain responsive and accessible.
- Future budgeting, savings goals, alerts, and dark mode capabilities are not yet confirmed as implemented product behavior.

## Evidence on Hand

- `README.md` documents the product purpose, authentication flow, architecture, security posture, deployment, and roadmap.
- `financetracker.client/src/app/app.routes.ts` confirms the authenticated Dashboard, Income, Expense, and Analytics routes.
- Existing dashboard and analytics components confirm transaction search/filtering, balance visibility, CSV/Excel export, category analytics, and empty states.
- No testimonials, customer studies, benchmarks, or other external proof are available in the repository; future work must not fabricate them.

## Product Principles

- Make everyday financial capture quick and understandable.
- Turn recorded transactions into useful insight without unnecessary complexity.
- Treat authentication and financial data handling as core product trust responsibilities.
- Preserve a clear, consistent vocabulary across transaction and analytics workflows.
- Be useful on responsive web layouts without sacrificing accessibility.

## Accessibility & Inclusion

The product is required to remain accessible and responsive across supported web screen sizes. The repository does not establish a more specific conformance target yet; future work should record one before treating it as a binding standard.
