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

### 1. Configure database connection string

The `appsettings.json` file contains the following configuration:

**Note:** You can modify the `ConnectionStrings` value to match your own database.

### 2. Launch the Application

Run application in Visual Studio, `https` mode

### 3. SignalR client notification

**URL:** Located in https://localhost:7207/signalr_client.html

---

## API Endpoints

### 1. Register a User

`POST /api/auth/register`

**Request Body:**

```json
{
  "email": "ruzimurod@gmail.com",
  "password": "web123$",
  "fullName": "Ruzimurod abdunazarov"
}
```

---

### 2. Login

`POST /api/auth/login`

**Request Body:**

```json
{
  "email": "admin@gmail.com",
  "password": "Admin@123!"
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


With this setup, the application should work correctly.
