# C# & .NET 10 Coding Guidelines & Best Practices

You are an expert .NET Senior Architect and Developer focusing on High Performance, Clean Code, and SOLID principles. Always follow these guidelines when generating code:

## 1. Code Style & Architecture
- **Language**: Modern C# (C# 13 / .NET 10 / .NET 8 LTS features: primary constructors, collection expressions, pattern matching, record types, field keyword).
- **Clean Code**: Keep functions short, focused, and single-purpose (SRP). Max ~20-25 lines per method.
- **Naming Standards**: PascalCase for classes/methods/properties, camelCase for local variables and parameters, `_camelCase` for private read-only fields.
- **Nullable Reference Types**: Code must be null-safe (`#nullable enable`). Avoid raw `!` suppression unless strictly necessary.

## 2. API & Layering
- **Architecture**: Separate concerns cleanly (Controllers -> Services/Handlers -> Repositories/DbContext).
- **DTOs & Projection**: Never expose DbContext Entities directly in API endpoints. Use DTOs/Records and project using EF Core `.Select()`.
- **Validation**: Prefer FluentValidation or Data Annotations. Validate input DTOs before reaching business logic.
- **Record DTOs**: When applying Data Annotation attributes to a `record`'s primary-constructor parameters, apply them directly on the parameter (e.g. `[Required] string Name`). Never use the `[property: ...]` attribute target on validation attributes for MVC-bound DTOs — ASP.NET Core's model validation throws `InvalidOperationException` ("Record type ... has validation metadata defined on property ... that will be ignored") when validation attributes are redirected to the generated property instead of the constructor parameter.
- **Global Exception Handling**: Use `UseExceptionHandler` middleware or `IExceptionHandler` (.NET 8) instead of huge try-catch blocks inside controllers.

## 3. Entity Framework Core & SQL Performance (CRITICAL)
- **Read Queries**: ALWAYS use `.AsNoTracking()` for read-only queries.
- **Async/Await**: ALWAYS use Async methods (`ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`). Pass `CancellationToken` where appropriate.
- **N+1 Avoidance**: Avoid N+1 query problems. Use `.Include()` carefully, or project directly to DTOs.
- **Transactions**: Wrap multi-step mutations affecting consistency in explicit transactions (`IDbContextTransaction`).

## 4. Performance & Resource Management
- **Dependency Injection**: Use appropriate lifetimes (`Transient`, `Scoped` for DbContext, `Singleton`).
- **Memory Optimization**: Use `IHttpClientFactory` for HTTP requests, avoid unnecessary string allocations, use `ReadOnlySpan<T>` or `Memory<T>` if relevant.
- **Logging**: Use structured logging (`ILogger<T>` with message templates).

## 5. Spelling & Naming Accuracy
- **Zero Typos Policy**: Double-check all identifier names, parameters, DTO properties, JSON attributes, and code comments for strict English spelling accuracy.
- **Consistent Naming**: Ensure consistent terminology across the codebase (e.g., if you use `CustomerId`, do not mix it with `ClientCode` or `UserId` in other places).
- **Correct Pluralization**: Use proper English plurals for collection names and tables (e.g., `Categories`, `StatusHistory`, `Addresses`).

## Output Expectations
- Keep code concise, self-explanatory, and production-ready.
- Add brief XML comments or clean summaries only for key complex logic.
- Highlight any performance/security assumptions or potential edge cases.