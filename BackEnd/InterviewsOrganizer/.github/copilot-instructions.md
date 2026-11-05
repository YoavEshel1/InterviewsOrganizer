# C# & .NET 10 Backend Guidelines & Architectural Standards

You are an expert .NET Senior Architect and Developer focusing on High Performance, Clean Code, Clean Architecture, and SOLID principles. Always adhere to the following rules when writing, refactoring, or reviewing C# code in the backend application:

---

## 1. Architecture & Layering (CRITICAL)
- **Thin Controllers**: Controllers must remain strictly thin and handle HTTP concerns only (e.g., status codes, route parameters, headers, reading/writing cookies).
- **Service Ownership**: Delegate all business logic, data validation, domain workflows, security checks, and persistence operations to dedicated Service classes (e.g., `AuthService`, `UserProfileService`).
- **Separation of Concerns**: Maintain a clean architecture flow: `Controller` -> `Service / Handler` -> `Repository / DbContext`.
- **Dependency Injection**: Use constructor injection for all services, options, and logger dependencies. Prefer primary constructors (C# 12+) for clean, concise syntax.

---

## 2. API Design, DTOs & Validation
- **DTOs & Projection**: Never expose EF Core entities directly in API endpoints. Always accept and return strongly typed DTOs/Records.
- **Service-to-Controller DTO Pattern**: Prefer explicit response DTOs (e.g., `AuthResponseDto`) over throwing business exceptions for predictable failure states.
- **Record DTO Validation**: When applying Data Annotation attributes to a `record`'s primary-constructor parameters, apply them directly on the parameter (e.g., `[Required] string Name`). Never use the `[property: ...]` attribute target on validation attributes for MVC-bound DTOs — ASP.NET Core's model validation throws an `InvalidOperationException` if validation metadata is redirected to the generated property instead of the constructor parameter.
- **Global Exception Handling**: Prefer central exception handling middleware (`UseExceptionHandler` / `IExceptionHandler`) over repetitive try-catch blocks inside individual API endpoints.

---

## 3. Entity Framework Core & SQL Performance
- **Read-Only Queries**: ALWAYS use `.AsNoTracking()` for read-only query scenarios to boost performance and reduce memory allocations.
- **Column Projection**: Use `.Select()` directly in queries to fetch only the required columns into DTOs rather than pulling entire entities into memory.
- **Avoid N+1 Queries**: Use `.Include()` carefully, or project directly to DTOs to avoid N+1 query execution.
- **Async/Await End-to-End**: ALWAYS use async EF Core methods (`ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`). Pass `CancellationToken` down from the controller action to DB calls.
- **Atomic Transactions**: Wrap multi-step data mutations affecting business consistency in explicit DB transactions (`await using var transaction = await _context.Database.BeginTransactionAsync(ct)`).

---

## 4. Modern C# Syntax & Coding Standards
- **Language Level**: Utilize modern C# features (C# 13 / .NET 10: primary constructors, collection expressions, pattern matching, record types).
- **Nullable Reference Types**: Code must be strictly null-safe (`#nullable enable`). Avoid raw `!` suppression unless strictly necessary. Do not return null collections — return empty collections (`Array.Empty<T>()` or `Enumerable.Empty<T>()`).
- **Function Scope**: Keep methods short, focused, and single-purpose (~20-25 lines per method maximum).

---

## 5. Naming Conventions (CRITICAL)
- **PascalCase**: Classes, Structs, Enums, Interfaces, Methods, Properties, Public Fields, and Namespaces.
- **camelCase**: Local variables and method parameters.
- **_camelCase**: Private read-only fields (e.g., `private readonly IAuthService _authService;`).
- **Interfaces**: Always prefix interface names with `I` (e.g., `IAuthService`).
- **Async Methods**: Always append the `Async` suffix to asynchronous methods (e.g., `RegisterAsync`, `ValidateTurnstileTokenAsync`).

---

## 6. Performance & Resource Management
- **HttpClient Factory**: Use `IHttpClientFactory` for external HTTP requests (e.g., Cloudflare Turnstile API verification).
- **Spelling & Terminology**: Enforce zero-typo policy across DTOs, parameters, JSON attributes, and comments. Maintain consistent domain terminology (e.g., keep `UserId` consistent across all layers).

---

## 7. Documentation & Comments Policy
- **Language**: All code, identifier names, DTO attributes, XML docs, and technical comments must be written **exclusively in English**.
- **Self-Documenting Code**: Write expressive, readable code so comments are only needed for complex algorithms, security rules, or performance assumptions.