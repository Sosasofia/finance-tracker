# Receipt library and secure Blob Storage access

## Overview

The receipt feature provides an authenticated receipt library backed by Azure
Blob Storage. Users can upload receipt images or PDFs, assign a recognizable
name, view saved files, process a saved receipt with Azure Document
Intelligence, and delete receipts after confirmation.

The storage container remains private. The frontend never receives storage
credentials and does not send the application JWT directly to Blob Storage.
Instead, the API authorizes the request and returns a short-lived,
read-only SAS URL for the selected receipt.

## User experience

The feature is available from the **Receipts** item in the main navigation.
The dashboard also exposes a camera action that opens the receipt library.

The library supports:

- Uploading `.jpg`, `.jpeg`, `.png`, and `.pdf` files up to 5 MB.
- Editing the display name before saving a receipt.
- Listing the authenticated user's saved receipts.
- Opening an image or PDF directly from a secure SAS URL.
- Processing a saved receipt to extract transaction data and open the expense
  form for review.
- Deleting a receipt through the shared confirmation dialog. The delete
  request is sent only after the user confirms the displayed receipt name.

Receipt scanning from the transaction form remains available independently:
the user can scan a file, review the extracted data, and choose whether to
save the original receipt.

## Request flow

### Upload and persistence

1. The Angular receipt service sends a `multipart/form-data` request to
   `POST /api/receipts`.
2. The controller obtains the authenticated user ID from
   `ICurrentUserService`; the browser cannot choose the owner.
3. The application validates file type, file size, and display name.
4. The Blob Storage service uploads the private blob using the path:

   ```text
   users/{userId}/receipts/{storageReceiptIdN}{extension}
   ```

5. Receipt metadata is persisted in the `receipt` table, including the
   display name, original file name, content type, size, upload date, and
   storage receipt ID.

### View with SAS

1. The frontend requests `GET /api/receipts/{receiptId}/url`.
2. The API verifies that the receipt belongs to the authenticated user.
3. The storage service creates a read-only SAS URL with a configurable short
   lifetime.
4. The frontend opens that URL in a new browser tab.
5. The JWT is not appended to the Blob Storage request.

Azure deployments use a user delegation SAS through managed identity. Local
development can use the configured Blob Storage connection string. The
default SAS lifetime is 15 minutes and is constrained by the backend
configuration.

### Process a saved receipt

1. The frontend sends `POST /api/receipts/{receiptId}/process`.
2. The API downloads the private blob after validating ownership.
3. The downloaded stream and its separate content length are passed to the
   receipt scanner.
4. The extracted data is returned to the frontend.
5. The frontend opens the expense form so the user can review and submit the
   transaction.

The scanner uses the content length returned by Blob Storage. It must not read
`Stream.Length`, because Azure's streaming response can be non-seekable and
may throw `NotSupportedException`.

### Delete

1. The user selects the delete action in the receipt library.
2. The shared `ConfirmDialogComponent` displays the receipt name and asks for
   confirmation.
3. If the user cancels, no API request is made.
4. If confirmed, the frontend sends
   `DELETE /api/receipts/{receiptId}`.
5. The API validates ownership, deletes the Blob Storage object, removes the
   metadata record, and refreshes the library.

## API contract

All receipt endpoints require authentication and are protected by the
`receipt-scanner-policy` rate-limit policy.

| Method | Route | Purpose |
| --- | --- | --- |
| `POST` | `/api/receipts` | Upload a named receipt as multipart form data. |
| `GET` | `/api/receipts` | List receipts owned by the current user. |
| `GET` | `/api/receipts/{receiptId}` | Stream a receipt through the API fallback. |
| `GET` | `/api/receipts/{receiptId}/url` | Return a short-lived read-only SAS URL. |
| `POST` | `/api/receipts/{receiptId}/process` | Scan a saved receipt and return extracted data. |
| `DELETE` | `/api/receipts/{receiptId}` | Delete the owned receipt and its blob. |
| `POST` | `/api/receipts/scan` | Scan an uploaded file without saving it. |

### Upload fields

`POST /api/receipts` expects `multipart/form-data`:

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| `file` | file | Yes | JPG, JPEG, PNG, or PDF; maximum 5 MB. |
| `displayName` | string | Yes | User-facing receipt name. |

### Secure URL response

`GET /api/receipts/{receiptId}/url` returns:

```json
{
  "url": "https://storage.example/...&sp=r&...",
  "expiresAt": "2026-10-31T23:56:00Z"
}
```

The URL should be treated as a temporary bearer capability. It must not be
stored in application state longer than needed or logged.

### Receipt list item

The list endpoint returns items with the following shape:

```json
{
  "id": "storage-receipt-id",
  "displayName": "October groceries",
  "originalFileName": "IMG_1024.jpg",
  "contentType": "image/jpeg",
  "size": 183421,
  "uploadedAt": "2026-10-31T22:30:00Z",
  "transactionId": null
}
```

## Backend components

- `ReceiptsController` exposes the authenticated HTTP endpoints.
- Receipt commands and queries are organized by use case under
  `FinanceTracker.Application.Features.Receipts`.
- `Receipt` stores metadata and the optional transaction association.
- `ReceiptRepository` scopes queries to the current user.
- `AzureBlobStorageService` uploads, downloads, deletes, and generates SAS
  URLs for private blobs.
- `ApplicationDbContext` maps the `receipt` table and its optional
  transaction relationship.
- The `AddReceipts` migration creates the required database objects.

In Development, the server applies pending migrations at startup. Production
deployments should apply migrations through the deployment process rather than
depending on application startup.

## Frontend components

- `ReceiptScannerService` wraps upload, list, SAS URL, process, delete, scan,
  and API fallback download calls.
- `ReceiptLibraryComponent` renders the upload form, receipt list, file
  actions, loading state, and notifications.
- `ReceiptsComponent` hosts the full receipt page and opens the expense form
  after processing.
- `ConfirmDialogComponent` provides the shared delete confirmation UX.
- The dashboard camera action and main navigation link to `/receipts`.

## Configuration and operations

Configure the backend Blob Storage settings through user secrets or
environment variables. Do not commit connection strings, account keys,
managed identity credentials, or SAS URLs.

Relevant settings include:

- `BlobStorage:ConnectionString`
- `BlobStorage:ContainerName`
- `BlobStorage:SasLifetimeMinutes`
- Azure managed identity and storage RBAC configuration

If a receipt cannot be opened, verify that:

1. The metadata row exists in the `receipt` table.
2. The blob uses the normalized storage ID without GUID dashes.
3. The API identity can read the container and create a user delegation SAS.
4. The SAS has not expired.
5. The authenticated user owns the requested receipt.

If uploads fail with `Invalid object name 'receipt'`, apply the
`AddReceipts` migration and restart the API before retrying.
