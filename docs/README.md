# FinanceTracker documentation

## Core documentation

- [Overview](0-Overview.md) - Project purpose, technology stack, architecture
  layers, authentication flow, local development, and roadmap.
- [Architecture](1-Architecture.md) - Detailed system architecture, data flow,
  layer boundaries, and deployment design.
- [API documentation](2-API_Documentation.md) - HTTP endpoints, request
  payloads, responses, and authentication requirements.
- [Setup guide](3-Setup.md) - Prerequisites, secrets, local startup, telemetry,
  and testing commands.

## Feature and change documentation

Feature-specific notes and implementation reports belong in
[`features/`](features/). This keeps the core documentation focused while
providing a home for detailed technical changes.

- [MediatR removal report](features/MediatR_Removal_Report.md) - Replacement of
  MediatR with the internal application request dispatcher.
- [Receipt library and SAS access](features/Receipts_Library_and_SAS.md) -
  Private Blob Storage receipts, secure viewing, processing, and deletion.

## Product and design references

- [Product overview](PRODUCT.md) - Product purpose, users, capabilities, and
  product principles.
- [Design notes](DESIGN.md) - UI and interaction design documentation.
