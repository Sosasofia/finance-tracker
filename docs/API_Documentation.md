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

