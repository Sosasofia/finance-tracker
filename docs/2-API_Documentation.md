# FinanceTracker API Documentation

## Base Information

**Base URL:**

```
/api
```

**Authentication:**
Most endpoints require a valid **JWT Bearer Token**.

```
Authorization: Bearer <token>
```

---

## Endpoint Overview

### User Management
| Method | Path | Summary |
| --- | --- | --- |
| POST | `/api/auth/google-login` | Authenticate or register a user using a Google ID token. |
| POST | `/api/auth/login` | Logs in a user using email and password credentials. |
| POST | `/api/auth/register` | Registers a new user with an email and password. |
| POST | `/api/auth/logout` | Clears the authentication cookie for the current session. |
| GET | `/api/auth/me` | Gets the current authenticated user's session. |
| GET | `/api/users/me/profile` | Gets the authenticated user's profile. |
| PUT | `/api/users/me/profile` | Updates the authenticated user's profile. |
| PUT | `/api/users/me/password` | Changes the authenticated user's password. |
| DELETE | `/api/users/me` | Deletes the authenticated user's account. |
| GET | `/api/users/me/preferences` | Gets the authenticated user's preferences. |
| PUT | `/api/users/me/preferences` | Updates the authenticated user's preferences. |
| POST | `/api/receipts` | Uploads and names a receipt for the authenticated user. |
| GET | `/api/receipts` | Lists the authenticated user's saved receipts. |
| GET | `/api/receipts/{receiptId}` | Streams a receipt through the API fallback. |
| GET | `/api/receipts/{receiptId}/url` | Returns a short-lived read-only SAS URL for a receipt. |
| POST | `/api/receipts/{receiptId}/process` | Scans a saved receipt and returns extracted data for review. |
| DELETE | `/api/receipts/{receiptId}` | Deletes an authenticated user's receipt. |
| POST | `/api/receipts/scan` | Extracts transaction data from an uploaded receipt. |
| GET | `/health` | Checks API and database health. |


### Categories
| Method | Path | Summary |
| --- | --- | --- |
| GET | `/api/categories` | Gets all available categories (global and user-specific). |
| POST | `/api/categories` | Adds a new category for the authenticated user. |
| GET | `/api/categories/{id}` | Get a specific category by its ID. |
| DELETE | `/api/categories/{id}` | Deletes a category associated with the authenticated user. |

### Payment Methods
| Method | Path | Summary |
| --- | --- | --- |
| GET | `/api/payment-methods` | Gets all payment methods available to the authenticated user. |
| POST | `/api/payment-methods` | Creates a new payment method for the authenticated user. |
| GET | `/api/payment-methods/{id}` | Gets a specific payment method by its ID. |
| DELETE | `/api/payment-methods/{id}` | Deletes an existing payment method owned by the user. |

### Transactions
| Method | Path | Summary |
| --- | --- | --- |
| POST | `/api/transactions` | Creates a new transaction for the authenticated user. |
| GET | `/api/transactions` | Gets all transactions for the authenticated user. |
| GET | `/api/transactions/{id}` | Gets a specific transaction by ID. |
| DELETE | `/api/transactions/{id}` | Deletes a transaction by ID (soft-delete). |
| PUT | `/api/transactions/{id}` | Updates an existing transaction. |
| PATCH | `/api/transactions/{id}/restore` | Restores a soft-deleted transaction. |
| GET | `/api/transactions/export/csv` | Exports the user's transactions as a CSV file. |
| GET | `/api/transactions/export/excel` | Exports the user's transactions as an Excel file. |

-----

## Authentication

This API uses **Bearer Token (JWT)** authentication. Most endpoints require an `Authorization` header with a valid token obtained from one of the `Auth` endpoints.

**Header Format:** `Authorization: bearer <YOUR_JWT_TOKEN>`

-----

## API Endpoint Details

### Auth

Endpoints for handling user authentication and registration.

#### `POST /api/auth/google-login`
🔓 *No authentication required.*

  * **Summary:** Authenticates or registers a user using a Google ID token.
  * **Description**: This endpoint accepts a Google ID token obtained on the client. It will either validate the token and sign in the existing user, or create a new user and return an authentication response. The response contains a JWT token and a user object.
  * **Sample Request**:
    ```json
    {
      "idToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6... (your token)"
    }
    ```
  * **Request Body**: `GoogleLoginRequest`
  * **Responses**:
      * `200 OK`: Returns the `AuthResponseDto` with a JWT and user details.
      ```
      {
        "token": "<jwt-token>",
        "user": {
          "id": "1a3b…",
          "email": "user@example.com"
        }
      }
      ```
      * `400 Bad Request`: The ID token is missing from the request.
      * `401 Unauthorized`: The Google token is invalid or authentication fails.

#### `POST /api/auth/login`
🔓 *No authentication required.*

  * **Summary:** Logs in a user using email and password credentials.
  * **Description**: Expects an `AuthRequestDto` containing a valid email and password. On success, returns an `AuthResponseDto` with a JWT token and user information.
  * **Sample Request**:
    ```json
    {
      "email": "user@example.com",
      "password": "P@ssw0rd!"
    }
    ```
  * **Request Body**: `AuthRequestDto`
  * **Responses**:
      * `200 OK`: Returns the `AuthResponseDto` with a JWT and user details.
      * `401 Unauthorized`: The email or password is incorrect.

#### `POST /api/auth/register`
🔓 *No authentication required.*

  * **Summary:** Registers a new user with an email and password.
  * **Description**: Creates a new user account and returns an `AuthResponseDto` when successful. The operation will fail if the provided email is already registered.
  * **Sample Request**:
    ```json
    {
      "email": "newuser@example.com",
      "password": "Str0ngP@ss!",
      "role": "User"
    }
    ```
  * **Request Body**: `AuthRegisterDto`
  * **Responses**:
      * `200 OK`: Returns the `AuthResponseDto` with a JWT and user details.
      * `400 Bad Request`: Email or password validation fails (e.g., short password).
      * `409 Conflict`: An account with that email already exists.

-----

### Current session

#### `POST /api/auth/logout`
🔓 *Authentication is optional; the endpoint clears the authentication cookie.*

  * **Summary**: Logs out the current session.
  * **Responses**:
      * `204 No Content`: The authentication cookie was cleared.

#### `GET /api/auth/me`
🔒 Requires JWT Authentication.

  * **Summary**: Gets the current authenticated user's session information.
  * **Responses**:
      * `200 OK`: Returns a `UserSessionDto`.
      * `401 Unauthorized`: Not authenticated or the session is invalid.

#### `UserSessionDto`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| id | string (uuid) | Yes | The user's unique identifier. |
| name | string | Yes | The user's display name. |
| email | string | Yes | The user's email address. |
| picture | string | No | The user's profile picture URL. |

-----

### User profile and account
🔒 Requires JWT Authentication.

#### `GET /api/users/me/profile`

  * **Summary**: Gets the authenticated user's profile.
  * **Responses**:
      * `200 OK`: Returns a `UserResponse`.
      * `401 Unauthorized`: Not authenticated.

#### `PUT /api/users/me/profile`

  * **Summary**: Updates the authenticated user's profile.
  * **Request Body**:
    ```json
    {
      "name": "Alex Smith",
      "profilePictureUrl": "https://example.com/profile.jpg"
    }
    ```
  * **Responses**:
      * `200 OK`: Returns the updated `UserResponse`.
      * `400 Bad Request`: The request payload is invalid.
      * `401 Unauthorized`: Not authenticated.

#### `PUT /api/users/me/password`

  * **Summary**: Changes the authenticated user's password.
  * **Request Body**:
    ```json
    {
      "currentPassword": "OldP@ssw0rd!",
      "newPassword": "NewP@ssw0rd!"
    }
    ```
  * **Responses**:
      * `200 OK`: The password was changed successfully.
      * `400 Bad Request`: The request payload is invalid or the current
        password is incorrect.
      * `401 Unauthorized`: Not authenticated.

#### `DELETE /api/users/me`

  * **Summary**: Deletes the authenticated user's account.
  * **Responses**:
      * `200 OK`: The account was deleted successfully.
      * `401 Unauthorized`: Not authenticated.

#### `UserResponse`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| email | string | Yes | The user's email address. |
| name | string | Yes | The user's display name. |
| picture | string | No | The user's profile picture URL. |
| role | string | Yes | The user's application role. |
| provider | string | Yes | The authentication provider. |
| createdAt | string (date-time) | Yes | When the account was created. |
| lastLoginAt | string (date-time) | Yes | When the user last signed in. |

-----

### User preferences
🔒 Requires JWT Authentication.

These endpoints read and update preferences for the authenticated user. The
user identity is taken from the JWT; it is not supplied in the request body.

#### `GET /api/users/me/preferences`

  * **Summary**: Gets the current user's preferences.
  * **Responses**:
      * `200 OK`: Returns a `UserPreferencesResponse`.
      * `401 Unauthorized`: Not authenticated.

#### `PUT /api/users/me/preferences`

  * **Summary**: Updates the current user's preferences.
  * **Request Body**:
    ```json
    {
      "defaultCurrency": "USD",
      "dateFormat": "MM/dd/yyyy"
    }
    ```
  * **Fields**:
      * `defaultCurrency` (string): The currency used as the user's default.
      * `dateFormat` (string): The date format used as the user's default.
  * **Responses**:
      * `200 OK`: Returns the updated `UserPreferencesResponse`.
      * `400 Bad Request`: The request payload is invalid.
      * `401 Unauthorized`: Not authenticated.

#### `UserPreferencesResponse`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| defaultCurrency | string | Yes | The user's default currency. |
| dateFormat | string | Yes | The user's preferred date format. |

-----

### Receipt scanning
🔒 Requires JWT Authentication.

#### `POST /api/receipts/scan`

  * **Summary**: Scans a receipt and extracts transaction information.
  * **Content type**: `multipart/form-data`
  * **Form field**:
      * `file` (file, required): A JPG, JPEG, PNG, or PDF receipt. Files must
        be no larger than 5 MB.
  * **Responses**:
      * `200 OK`: Returns an `ExtractedReceiptDto` with the detected receipt
        details.
      * `400 Bad Request`: No file was uploaded, the file is too large, or the
        file type is not supported.
      * `401 Unauthorized`: Not authenticated.
      * `500 Internal Server Error`: Receipt processing failed.

#### `ExtractedReceiptDto`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| transactionDate | string | No | Date detected on the receipt. |
| amount | number | No | Total amount detected on the receipt. |
| merchantName | string | No | Merchant name detected on the receipt. |
| categoryId | string (uuid) | No | Matching category identifier, when available. |
| categoryConfidence | number | No | Confidence score for the category match. |
| paymentMethodId | string (uuid) | No | Matching payment method identifier, when available. |
| paymentType | string | No | Payment type detected on the receipt. |
| paymentMethodConfidence | number | No | Confidence score for the payment method match. |
| rawCategoryText | string | No | Original category text extracted from the receipt. |
| rawPaymentMethodText | string | No | Original payment method text extracted from the receipt. |
| rawContent | string | No | Raw content extracted from the receipt. |
| lineItems | array of strings | Yes | Line items extracted from the receipt. |

-----

### Receipt storage
🔒 Requires JWT Authentication.

Receipt files are stored in a private Azure Blob Storage container. The API
never accepts a client-supplied blob name, user ID, or public storage URL.

#### `POST /api/receipts`

Uploads a JPG, JPEG, PNG, or PDF receipt for the authenticated user. The
maximum size is 5 MB. The multipart form must contain `file` and a required
`displayName` (maximum 120 characters). The response contains an opaque receipt
identifier that can later be sent as `receiptUrl` when creating a transaction.

#### `GET /api/receipts`

Lists saved receipts owned by the authenticated user, including their display
name, original file name, content type, size, upload date, and optional
transaction association.

#### `GET /api/receipts/{receiptId}`

Streams a previously uploaded receipt when it belongs to the authenticated
user. Files are not publicly accessible. This endpoint remains the API download
fallback for clients that cannot use a direct blob URL.

#### `GET /api/receipts/{receiptId}/url`

Returns a short-lived, read-only SAS URL for a receipt owned by the
authenticated user:

```json
{
  "url": "https://storage-account.blob.core.windows.net/receipts/...",
  "expiresAt": "2026-10-31T20:42:44Z"
}
```

The frontend can use `url` directly for an image or download without sending
the application JWT to Azure Storage. The URL is never persisted or logged by
the API. `BlobStorage:SasLifetimeMinutes` controls the lifetime and must be
between 1 and 60 minutes.

#### `POST /api/receipts/{receiptId}/process`

Processes an already saved receipt through the existing scanner and returns
extracted fields. It does not create a transaction; the frontend should open
the expense form for user review and confirmation.

#### `DELETE /api/receipts/{receiptId}`

Deletes a receipt owned by the authenticated user and returns `204 No Content`.
Uploads are initially marked as orphaned until a future transaction-association
flow marks them as attached. Orphan cleanup is intentionally a separate
retention process and is not performed during normal requests.

-----

### Health

#### `GET /health`

  * **Summary**: Checks whether the API and configured database are healthy.
  * **Authentication**: No authentication required.
  * **Responses**:
      * `200 OK`: The API and database health checks passed.
      * `503 Service Unavailable`: One or more health checks failed.

-----

### Category
🔒 Requires JWT Authentication.

Endpoints for managing user categories. 

#### `GET /api/categories`

  * **Summary**: Gets all categories for the currently authenticated user.
  * **Description**: Retrieves all categories that belong to the user identified by the current authentication context.
  * **Responses**:
      * `200 OK`: Returns an array of `CategoryDto`.
      * `401 Unauthorized`: Not authenticated.
      * `403 Forbidden`: Not authorized.

#### `POST /api/categories`

  * **Summary:** Creates a new category for the currently authenticated user.
  * **Description**: Creates a new category using the provided `CreateCategoryDto`. The operation associates the created category with the currently authenticated user.
  * **Request Body**: `CreateCategoryDto`
  * **Responses**:
      * `201 Created`: Returns the newly created `CategoryDto` and a Location header.
      * `400 Bad Request`: The request payload is invalid.
      * `401 Unauthorized`: Not authenticated.
      * `403 Forbidden`: Not authorized.

#### `GET /api/categories/{id}`

  * **Summary**: Gets a specific category by its ID.
  * **Parameters**:
      * `id` (GUID, path): The unique identifier of the category.
  * **Responses**:
      * `200 OK`: Returns the specified `CategoryDto`.
      * `401 Unauthorized`: Not authenticated.
      * `403 Forbidden`: Not authorized.
      * `404 Not Found`: The category with the specified ID is not found.

#### `DELETE /api/categories/{id}`

  * **Summary**: Deletes a specific category belonging to the current user.
  * **Description**: Deletes the category identified by the provided GUID. The operation will only succeed if the category belongs to the currently authenticated user.
  * **Parameters**:
      * `id` (GUID, path): The unique identifier of the category to delete.
  * **Responses**:
      * `204 No Content`: Indicates the category was successfully deleted.
      * `401 Unauthorized`: Not authenticated.
      * `403 Forbidden`: Not authorized.
      * `404 Not Found`: The category with the specified ID is not found.

-----

### PaymentMethod
🔒 Requires JWT Authentication.

Endpoints for managing user payment methods.

#### `GET /api/payment-methods`

  * **Summary**: Retrieves all payment methods available to the authenticated user.
  * **Description**: Returns a list of payment methods (e.g., bank accounts, cards) associated with the current user.
  * **Responses**:
      * `200 OK`: A list of user-owned `PaymentMethodDto`.
      * `401 Unauthorized`: Not authenticated.
      * `403 Forbidden`: Not authorized.

#### `POST /api/payment-methods`

Creates a new payment method for the authenticated user.

  * **Description**: The request must include a valid `CreatePaymentMethodDto`. The created payment method will be associated with the currently authenticated user.
  * **Request Body**: `CreatePaymentMethodDto`
  * **Responses**:
      * `201 Created`: Payment method created successfully.
      * `400 Bad Request`: The request payload is invalid.
      * `401 Unauthorized`: Not authenticated.
      * `403 Forbidden`: Not authorized.

#### `GET /api/payment-methods/{id}`

  * **Summary**: Retrieves a single payment method by its identifier.
  * **Parameters**:
      * `id` (GUID, path): The unique identifier of the payment method.
  * **Responses**:
      * `200 OK`: Returns the payment method.
      * `401 Unauthorized`: Not authenticated.
      * `403 Forbidden`: Not authorized.
      * `404 Not Found`: If no payment method with the given id exists.

#### `DELETE /api/payment-methods/{id}`

  * **Summary**: Deletes an existing payment method.
  * **Parameters**:
      * `id` (GUID, path): The unique identifier of the payment method to delete.
  * **Responses**:
      * `204 No Content`: Payment method deleted successfully.
      * `401 Unauthorized`: Not authenticated.
      * `403 Forbidden`: Not authorized.
      * `404 Not Found`: If the payment method does not exist.

-----

### Transaction
🔒 Requires JWT Authentication.

Endpoints for managing user transactions. 

#### `POST /api/transactions`

  * **Summary:** Creates a new transaction for the authenticated user.
  * **Request Body**: `CreateTransactionDto`
  * **Responses**:
      * `201 Created`: Created transaction response.
      * `200 OK`: Returns the `TransactionResponse`.
      * `400 Bad Request`: If the transaction data is invalid.

#### `GET /api/transactions`

  * **Summary**: Retrieves all transactions for the authenticated user.
  * **Responses**:
      * `200 OK`: Returns an array of `TransactionResponse`.
      * `404 Not Found`: If no transactions are found for the user.

  > ⚠️ Note: Pagination will be added in a future version.

#### `GET /api/transactions/{id}`

  * **Summary**: Retrieves a specific transaction by id for the authenticated user.
  * **Parameters**:
      * `id` (GUID, path): Transaction identifier.
  * **Responses**:
      * `200 OK`: Returns the `TransactionResponse`.
      * `404 Not Found`: If the transaction is not found.

#### `DELETE /api/transactions/{id}`

  * **Summary**: Deletes the specified transaction only if it belongs to the authenticated user.
  * **Details**: 
      * The transaction is **soft-deleted**, it can be later restored.
  * **Parameters**:
      * `id` (GUID, path): Transaction identifier.
  * **Responses**:
      * `204 No Content`: Transaction deleted successfully.
      * `200 OK`: (Alternative success response).
      * `400 Bad Request`: If deletion failed.

##### **Soft-delete behavior**

* `DELETE` marks the transaction as deleted
* Deleted items **do not appear in GET results**
* Can be restored using `PATCH /restore`


#### `PUT /api/transactions/{id}`

  * **Summary**: Updates a transaction for the authenticated user.
  * **Parameters**:
      * `id` (GUID, path): Transaction identifier.
  * **Request Body**: `UpdateTransactionDto`
  * **Responses**:
      * `200 OK`: Returns the updated transaction.
      * `400 Bad Request`: If update data is invalid or update failed.

#### `PATCH /api/transactions/{id}/restore`

  * **Summary**: Restores a previously deleted transaction.
  * **Parameters**:
      * `id` (GUID, path): Transaction identifier.
  * **Responses**:
      * `200 OK`: Returns the restored transaction.
      * `400 Bad Request`: If restoration fails or transaction not found.

#### `GET /api/transactions/export/csv`

  * **Summary**: Exports transactions as CSV within the specified date range.
  * **Query Parameters**:
      * `dateFrom` (datetime, query): Start date (inclusive).
      * `dateTo` (datetime, query): End date (inclusive).
  * **Responses**:
      * `200 OK`: Returns a CSV file.

    #### Response Headers
      ```
      Content-Type: text/csv
      Content-Disposition: attachment; filename="transactions.csv"
      ```

#### `GET /api/transactions/export/excel`

  * **Summary**: Exports transactions as an Excel file within the specified date range.
  * **Query Parameters**:
      * `dateFrom` (datetime, query): Start date (inclusive).
      * `dateTo` (datetime, query): End date (inclusive).
  * **Responses**:
      * `200 OK`: Returns an Excel file.
    #### Response Headers
      ```
      Content-Type: text/csv
      Content-Disposition: attachment; filename="transactions.csv"
      ```

-----

## Models (Schemas)

### `AuthRegisterDto`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| email | string (email) | **Yes** | |
| password | string | **Yes** | Min length: 8 |
| role | string | No | |

### `AuthRequestDto`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| email | string (email) | **Yes** | |
| password | string | **Yes** | Min length: 8 |

### `AuthResponseDto`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| token | string | No | The JWT Bearer token. |
| user | `UserResponse` | No | Basic user information. |

### `CategoryDto`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| id | string (uuid) | No | |
| name | string | No | |
| type | string | No | |
| userId | string (uuid) | No | |

### `CreateCategoryDto`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| name | string | No | Max length: 15 |

### `CreatePaymentMethodDto`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| name | string | No | |
| type | string | No | |

### `CreateTransactionDto`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| amount | number (double) | **Yes** | Min value: 0.01 |
| name | string | **Yes** | Min length: 3, Max length: 100 |
| description | string | No | Max length: 500 |
| date | string (date-time) | **Yes** | |
| notes | string | No | |
| receiptUrl | string | No | |
| type | `TransactionType` | No | |
| categoryId | string (uuid) | **Yes** | |
| paymentMethodId | string (uuid) | **Yes** | |
| isCreditCardPurchase | boolean | No | |
| installment | `InstallmentDto` | No | |
| isReimbursement | boolean | No | |
| reimbursement | `ReimbursementDto` | No | |

### `ErrorResponse`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| title | string | No | |
| status | integer | No | |
| detail | string | No | |

### `GoogleLoginRequest`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| idToken | string | No | The ID token from Google. |

### `InstallmentDto`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| number | integer | **Yes** | Min: 1, Max: 12 |
| interest | integer | No | |

### `InstallmentResponse`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| id | string (uuid) | No | |
| installmentNumber | integer | No | |
| amount | number (double) | No | |
| dueDate | string (date-time) | No | |
| paymentDate | string (date-time) | No | |
| isPaid | boolean | No | |

### `ReimbursementDto`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| amount | number (double) | **Yes** | Min value: 0.01 |
| date | string (date-time) | **Yes** | |
| reason | string | No | |

### `TransactionResponse`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| id | string (uuid) | No | |
| amount | number (double) | No | |
| name | string | No | |
| description | string | No | |
| date | string (date-time) | No | |
| notes | string | No | |
| receiptUrl | string | No | |
| type | `TransactionType` | No | |
| categoryId | string (uuid) | No | |
| paymentMethodId | string (uuid) | No | |
| category | `CategoryDto` | No | |
| installments | array (`InstallmentResponse`) | No | |
| reimbursement | `ReimbursementDto` | No | |

### `TransactionType`

Enum (string)

  * `Income`
  * `Expense`

### `UpdateTransactionDto`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| amount | number (double) | No | Min value: 0.01 |
| name | string | No | Min: 3, Max: 100 |
| description | string | No | Max: 500 |
| date | string (date-time) | No | |
| notes | string | No | Max: 1000 |
| receiptUrl | string | No | |
| type | `TransactionType` | No | |
| categoryId | string (uuid) | No | |
| paymentMethodId | string (uuid) | No | |

### `UserResponse`

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| id | string | No | |
| email | string | No | |


--------------------------------------------------------------
