# Test Strategy

This project uses a **clear separation between Unit Tests and Integration Tests
**, following common practices used in .NET APIs.

The goal is to keep tests:

- fast and deterministic at the unit level
- realistic and representative at the integration level

---

## 🧪 Unit Tests

**Purpose:**  
Validate business rules and validation logic in isolation.

**Characteristics:**

- Do not start the web server
- Do not use HTTP
- Focus on services and validators

**Database strategy:**

- `EntityFrameworkCore.InMemory`
- Created via a dedicated `TestDbContextFactory`

**Why InMemory here?**

- Extremely fast
- Ideal for pure business logic
- No need to simulate real SQL behavior at this level

This layer answers questions like:

- “Does this service enforce the correct rules?”
- “Does this validator reject invalid input?”

---

## 🌐 Integration Tests

**Purpose:**  
Validate the full HTTP pipeline, including:

- routing
- authentication
- authorization
- middleware
- persistence

**Characteristics:**

- Uses `WebApplicationFactory`
- Executes real HTTP requests
- Exercises controllers end-to-end

**Database strategy:**

- SQLite **in-memory**
- Single shared connection kept open during the test run

**Why SQLite in-memory instead of EF InMemory?**

- SQLite is a real relational database
- Foreign keys, constraints, and SQL behavior are respected
- Much closer to production behavior than EF InMemory
- No external dependencies (Docker / containers) required

This layer answers questions like:

- “Is this endpoint correctly protected?”
- “Does authentication + session validation behave as expected?”
- “Does the controller return the correct HTTP status codes?”

---

## 🧱 Integration Test Base Infrastructure

To avoid duplication and keep tests readable, integration tests share a common
base class that provides:

- `CreateClient(authenticated: bool)`
- Database reset helpers
- User and session seeding helpers

This ensures:

- clean database state per test
- explicit test intent
- no hidden coupling between tests

---

## 🔐 Authentication in Integration Tests

Integration tests use a **custom authentication handler** (`TestAuthHandler`)
that simulates an authenticated user.

- Authentication is triggered by the presence of an `Authorization` header
- The handler injects predefined claims (UserId, Email, JTI)
- Session validity is enforced by the real session middleware

This allows precise testing of scenarios such as:

- unauthenticated access
- authenticated user without session
- authenticated user with valid session

---

## 🧠 Design Rationale

This testing strategy intentionally mirrors real-world API testing:

- Unit tests remain fast and isolated
- Integration tests remain realistic and meaningful
- Database behavior is never mocked at the HTTP layer

Each decision was made to improve confidence, clarity, and long-term
maintainability.

---

## ✅ Summary

| Test Type   | Database Strategy | Purpose                     |
|-------------|-------------------|-----------------------------|
| Unit Tests  | EF Core InMemory  | Business rules & validation |
| Integration | SQLite In-Memory  | HTTP pipeline & persistence |

This approach balances speed, realism, and clarity.
