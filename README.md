# YouNa School – Backend API

> 🚧 **Project Status**: Under Active Development  
> This project is actively being developed. Features, structure, and documentation are subject to change as the system evolves.

## Overview

YouNa School is a modular, backend-focused application designed to support an online learning platform. The goal is to provide a clean, scalable, and maintainable architecture for managing users, courses, paid lectures, wallets, and related educational workflows.

The project follows modern .NET best practices, emphasizing:

- **Clear separation of concerns**
- **Domain-driven design principles**
- **Testability and extensibility**
- **Professional validation, error handling, and result patterns**

## Architecture

The solution follows a **Clean Architecture + Modular Monolith** approach, optimized for long-term maintainability and scalability. Each module represents a distinct business capability and owns its domain logic end-to-end.

### High-Level Layers

| Layer | Responsibility |
|-------|----------------|
| **API / Presentation Layer** | Exposes HTTP endpoints (Controllers), handles request/response mapping, delegates business logic to the Application layer. |
| **Application Layer** | Contains use cases and application services, implements validation, result patterns, and business workflows. |
| **Domain Layer** | Core business rules, entities, value objects, aggregates, and domain events. Independent of frameworks. |
| **Infrastructure Layer** | Database access (ORM, repositories, Unit of Work), external services, and technical implementations. |

### Project Structure

```
src/
 ├─ Modules/
 │   ├─ Users/                    # User accounts & identity
 │   ├─ Courses/                  # Courses and lectures
 │   ├─ Wallet/                   # Wallet & balance management
 │   ├─ Payments/                 # Payment & purchase workflows
 │   ├─ Quizzes/                  # Assessments and evaluations
 │   ├─ Chat/                     # Communication system
 └─ Shared/                   # Cross-cutting abstractions
```

---

## Modules Overview

Each module is self-contained and includes its own Domain, Application, and Infrastructure concerns.

| Module | Description |
|--------|-------------|
| **Users** | Handles user accounts, authentication, authorization, and role/permission management. |
| **Courses** | Manages courses, lectures (free & paid), enrollment workflows, and student progress foundations. |
| **Wallet** | Manages user balances, credit/debit transactions, balance consistency, and auditable wallet history. |
| **Payments** | Handles payment processing, purchase workflows, and transaction coordination. |
| **Quizzes** | Manages quizzes, questions, student submissions, attempts, scoring, and result calculation. |
| **Chat** | Enables communication between instructors/assistants and students with one-to-one/group conversations. |
| **Shared** | Contains cross-module contracts, domain events, common interfaces, and shared value objects. |

---

## Key Concepts

### Modular Design
Each feature (e.g., Wallet, Lectures, Payments) is developed as an isolated module with its own:
- Domain logic
- Application services
- Persistence configuration

This keeps the system flexible and easy to extend.

### Validation
Input validation is handled at the **Application layer**, typically using **FluentValidation**. This ensures:
- Controllers remain thin
- Business rules are enforced consistently
- Clear and structured error responses

### Result Pattern
Instead of throwing exceptions for expected failures, the project uses a **Result pattern** to:
- Represent success and failure explicitly
- Return meaningful domain and validation errors
- Improve readability and predictability of application flows

### Auditing & Time Abstraction
The system supports auditing concepts (e.g., created/updated timestamps) and abstracts time using a **system clock interface**. This:
- Improves testability
- Avoids direct dependency on system time

### Persistence
Data access is handled via **repositories** and a **Unit of Work** pattern:
- `SaveChangesAsync` is centralized and transaction-safe
- Paid lecture data, wallet transactions, and user progress are persisted through the Infrastructure layer

---

## Current State

### 🚧 Under Active Development
This repository represents an evolving system. Expect:
- Ongoing refactoring
- Incomplete features
- Changing APIs and schemas

The focus is on **getting the architecture right first**, before locking down behavior.

**The project is not production-ready yet.** Some areas may be incomplete or refactored over time, including:
- Business rules
- Database schema
- API contracts
- Error handling and edge cases

---

## Roadmap (High-Level)

- [ ] Complete core domain models
- [ ] Finalize wallet and payment workflows
- [ ] Improve validation and error handling
- [ ] Add automated tests (unit, integration)
- [ ] Enhance documentation and examples
- [ ] Implement real-time features (Chat module)
- [ ] Add monitoring and logging
- [ ] Prepare for production deployment

---

## Contributing

This project is currently under active development. Contributions, refactoring, and feedback are welcome as the architecture stabilizes.

Please check the **Issues** and **Projects** tabs for ongoing work and discussion.

---

## License

License information will be added once the project reaches a stable release.

---

> If you are reading this early—welcome! The foundation is being built with care, and more is coming soon.
