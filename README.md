# TodoList API – ASP.NET Core REST API Study (.NET 10)

## 📌 Project Goal

This project was developed as a **practical study of ASP.NET Core (.NET 10 LTS)
**,
with a focus on building secure REST APIs, clean code organization, and
**consolidating the fundamentals of the modern .NET platform**.

The main goal is to demonstrate progressive mastery of the .NET ecosystem
through
the application of solid backend concepts such as authentication, middleware,
validation, data access, and automated API documentation.

---

## 🧩 Overview

This API is designed with performance, simplicity, and clear separation of
responsibilities in mind, serving as a proof of concept.

The project applies responsibility separation principles inspired by
**Clean Architecture**, security based on **JWT with active session validation
**,
and automated API documentation rendered via **Scalar**.

---

## 🛠️ Tech Stack

- **Runtime**: .NET 10 LTS (C# 12+)
- **Persistence**: MySQL with Entity Framework Core (Code First)
- **Security**: JWT Bearer + active session control via middleware
- **Validation**: FluentValidation integrated into the model binding pipeline
- **Documentation**: OpenAPI generated via `Microsoft.AspNetCore.OpenApi` and
  rendered using Scalar
- **Utilities**:
    - DotNetEnv for environment variable management
    - BCrypt.Net-Next for password hashing

---

## 🧱 Architecture & Implemented Features

- [x] **Global Error Handling**  
  Implementation of `IExceptionHandler` to intercept unhandled exceptions and
  return consistent JSON payloads, preventing internal detail leakage in
  production environments.

- [x] **Options Pattern**  
  Centralized and validated configuration of sensitive settings (JWT) during
  application startup.

- [x] **Custom OpenAPI Infrastructure**  
  Use of `DocumentTransformer` for explicit JWT security scheme configuration
  within the .NET 10 OpenAPI pipeline.

- [x] **Active Session Validation**  
  Middleware positioned between Authentication and Authorization to validate
  tokens based on persisted sessions (JTI), enabling real-time token revocation.

- [x] **Identity Module**  
  User registration, JWT authentication with persistent sessions, authenticated
  profile management, password change with session invalidation, and the `/me`
  endpoint for retrieving the authenticated user context.

- [ ] **Task Management**  
  CRUD endpoints for tasks strictly scoped to the authenticated user.

---

## 🔐 Authentication Flow

1. The user logs in and receives a **JWT**.
2. The token contains a unique **JTI (JWT ID)**.
3. The session is persisted in the database.
4. On each authenticated request:
    - The JWT is validated.
    - Middleware verifies whether the session is still active.
5. Revoked tokens or expired sessions result in **HTTP 401 (Unauthorized)**.

---

## 📄 Endpoints (Examples)

- `POST /api/auth/register` – User registration
- `POST /api/auth/login` – Authentication and JWT issuance
- `POST /api/auth/logout` – Active session revocation (authenticated)
- `GET /api/me` – Returns the authenticated user data from the JWT context,
  used for session validation and frontend initialization

> The full list of endpoints is available via Scalar.

---

## 🧪 Tests

This project adopts a **layered testing strategy**, separating unit tests from
integration tests to balance speed, isolation, and realism.

- **Unit tests** focus on business rules and validation logic.
- **Integration tests** validate the full HTTP pipeline, including
  authentication,
  authorization, middleware, and persistence.

📘 A detailed explanation of the testing strategy is available at:  
👉 **[tests/README.md](tests/README.md)**

---

## 🚀 Running the Project

### 1. Prerequisites

- .NET SDK 10 installed
- MySQL running locally or via Docker

### 2. Environment Configuration

The project uses environment variables centralized at the **repository root**
(before the `src/` folder).

Use the `.env.example` file as a reference and create a `.env` file:

```bash
cp .env.example .env
```

Example variables:

```env
DB_HOST=localhost
DB_PORT=3306
DB_NAME=todo_list
DB_USER=root
DB_PASSWORD=password

Jwt__Key=secret-key
Jwt__Issuer=TodoList.API
Jwt__Audience=TodoList.Client
Jwt__ExpireMinutes=60
```

### 3. Database Migrations

```
dotnet ef database update
```

### 4. Running the Application

```
dotnet watch run
```

### 5. API Documentation

The API documentation UI is available in the **Development** environment:

```
/scalar
```

---

## 🐳 Docker (Optional)

This repository includes auxiliary files (`compose.yaml`) to run the environment
using Docker, simplifying dependency setup such as the database.

---

## ✅ Quality Standards

* **Strong Typing**: DTOs with explicit validation and safe initialization (
  `required` / `init` where applicable).
* **Security**:

    * Password hashing using BCrypt
    * Zero-leak policy for stack traces in production
* **Extensibility**:

    * Decoupled OpenAPI configuration via `DocumentTransformer`
    * Custom middlewares for cross-cutting business rules

---

## 📚 Final Notes

This project is intended for **fundamental consolidation and hands-on
exploration** of the modern .NET platform, serving as a foundation for
continuous evolution and deeper exploration of real-world production scenarios.
