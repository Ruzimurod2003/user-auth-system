# User Authentication System

This project provides functionality for user registration, login, token refresh, and user management (admin-only) using `ASP.NET Core`, `PostgreSQL`, and `Dapper`. The frontend consists of an `index.html` page served via `Live Server`.

## Technologies

* **Backend:** ASP.NET Core Web API
* **Frontend:** HTML (`index.html`)
* **ORM:** Dapper
* **Database:** PostgreSQL
* **Tokens:** JWT and Refresh Token
* **SignalR:** Real-time notifications (when a user is deleted)

---

## Getting Started

### 1. Launch the Frontend

Run the `index.html` file using the `Live Server` extension in VS Code:

```bash
http://localhost:5500/index.html
```

### 2. Launch the Backend

The `appsettings.json` file contains the following configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Key": "3249f3f0-8b1e-4ebb-a8ee-1e40b0e90034",
    "Issuer": "user auth system",
    "Expire": "7"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres"
  }
}
```

**Note:** You can modify the `ConnectionStrings` value to match your own database.

**To start the backend:**

```bash
dotnet run
```

---

## API Endpoints

### 1. Register a User

`POST /api/auth/register`

**Request Body:**

```json
{
  "email": "user@example.com",
  "password": "string",
  "fullName": "User Full Name"
}
```

---

### 2. Login

`POST /api/auth/login`

**Request Body:**

```json
{
  "email": "user@example.com",
  "password": "string"
}
```

**Response:**

```json
{
  "token": "jwt_token",
  "refreshToken": "refresh_token"
}
```

---

### 3. Refresh Token

`POST /api/auth/refresh-token`

**Request Body:**

```json
{
  "refreshToken": "refresh_token"
}
```

---

### 4. Get All Users (Admin Only)

`GET /api/user/users`

**Header:**
`Authorization: Bearer {token}`

---

### 5. Delete a User (Admin Only)

`DELETE /api/user/delete/{id}`

**Header:**
`Authorization: Bearer {token}`

**SignalR sends a notification to all connected clients:**

```json
{
  "event": "UserDeleted",
  "data": { "userId": 5 }
}
```

---

## CORS Issue

If you open `http://localhost:5500/index.html` via `Live Server`, you must add this origin to CORS in your backend’s `Program.cs` file:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

With this setup, the application should work correctly.
