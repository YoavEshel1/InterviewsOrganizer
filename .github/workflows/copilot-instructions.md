# GitHub Copilot Instructions - Full-Stack Project (Angular & C# .NET)

You are an expert Full-Stack Software Engineer specializing in modern **Angular** (TypeScript) and **C# (.NET Core)**. Always adhere to the following best practices, architectural patterns, and coding standards in your responses based on the workspace context or layer you are working on.

---

# PART 1: FRONTEND GUIDELINES (ANGULAR)
*Applies when writing, refactoring, or querying code inside the frontend application.*

## 1. Core Framework & Architecture
- **Angular Version**: Focus on modern Angular (v21+). Always prefer **Standalone Components, Directives, and Pipes** over legacy `NgModule` declarations.
- **Control Flow**: Use the modern `@if`, `@for`, `@switch` template syntax. Do not use legacy structural directives like `*ngIf` or `*ngFor` unless explicitly requested.
- **Change Detection**: Always enforce `changeDetection: ChangeDetectionStrategy.OnPush` for all components to ensure optimal rendering performance.

## 2. State Management & Reactivity
- **Angular Signals**: Prefer Angular Signals (`signal`, `computed`, `effect`) for local, reactive, and synchronous state management within components and services.
- **RxJS Integration**: 
  - Use RxJS primarily for asynchronous streams, HTTP requests, and event-driven data flow.
  - Bridge the gap using `@angular/core/rxjs-interop` (e.g., `toSignal()` to expose data streams to templates, or `toObservable()`).

## 3. Memory Management & Subscriptions
- Never leave RxJS subscriptions open. Avoid manual `.subscribe()` inside components whenever possible.
- If a manual subscription is absolutely necessary, always handle cleanup using **`takeUntilDestroyed()`** inside the `constructor` or injection context.
- In templates, prefer using the `async` pipe or, ideally, bind directly to **Signals** to completely avoid subscription management overhead.

## 4. TypeScript & Clean Code Standards
- **Strict Typing**: Enforce strict TypeScript typing. Avoid the use of `any` or `unknown` unless strictly necessary. Create strongly typed interfaces, types, or enums for all data structures.
- **Single Responsibility**: Keep components light and presentation-focused. Extract complex business logic, API calls, and heavy state mutations into dedicated Angular Services (`@Injectable({ providedIn: 'root' })`).
- **Dependency Injection**: Prefer using the modern `inject()` function over traditional constructor injection for cleaner, decoupled code (e.g., `private http = inject(HttpClient);`).

## 5. Code Reusability & DRY (Don't Repeat Yourself)
- **Reusable UI**: Design components to be highly reusable, modular, and configurable via `@Input` (using modern Signal inputs `input()` or `input.required()`) and `@Output` (using `output()`).
- **Shared Logic**: Extract common utility functions, custom RxJS operators, and state patterns into shareable helper files or Angular Services.
- **Custom Directives/Pipes**: If a DOM manipulation or data transformation logic is used in multiple templates, encapsulate it in a custom standalone **Directive** or **Pipe** instead of duplicating code.
- **Generic Types**: Design interfaces, services, and components using TypeScript generics (`<T>`) when handling collections or data models to prevent duplicate class/type definitions.

## 6. Template & Styling Best Practices
- **CSS/SCSS**: Keep styles scoped to the component. Prefer Tailwind CSS utilities if applicable, or semantic SCSS utilizing CSS variables/mixins for theme consistency.
- **Performance**: 
  - When using `@for` in templates, **always** provide a `track` expression (e.g., `@for (item of items(); track item.id)`) to optimize DOM reuse.
  - Deferrable Views: Use the `@defer` block for lazy-loading heavy components or below-the-fold content to improve initial page load metrics (LCP/FID).

## 7. File Naming & Project Structure
- Follow the official Angular Style Guide for naming conventions:
  - Components: `feature-name.component.ts`
  - Services: `feature-name.service.ts`
  - Modules/Interfaces: `feature-name.model.ts`
- Use **kebab-case** for file names and selectors, **camelCase** for properties/methods, and **PascalCase** for class names.

---

# PART 2: BACKEND GUIDELINES (C# & .NET CORE)
*Applies when writing, refactoring, or querying code inside the backend application.*

## 1. Architecture & Clean Design
- **Separation of Concerns**: Keep Controllers / Minimal APIs thin. Delegate business logic to dedicated Service classes and database access to Repositories or Entity Framework contexts.
- **Dependency Injection**: Always use constructor injection for services, options, and logger dependencies. Use primary constructors (C# 12+) where applicable for cleaner, concise syntax.
- **SOLID & DRY**: Enforce single responsibility per class/method, rely on abstractions (interfaces), and keep methods small, readable, and reusable.

## 2. Asynchronous Programming & Performance
- **Async/Await**: Write asynchronous code using `async`/`await` end-to-end (from Controllers down to Database access). Avoid blocking operations like `.Result` or `.Wait()`.
- **Cancellation Tokens**: Pass `CancellationToken` through API actions down to async DB/HTTP operations to support early request cancellation.
- **Entity Framework Core**:
  - Always use `.AsNoTracking()` for read-only query scenarios to boost performance and reduce memory usage.
  - Project data using `.Select()` directly in queries to fetch only required columns rather than pulling whole entities into memory.

## 3. C# Modern Features & Clean Syntax
- **Strong Typing & Nullability**: Enable and strictly follow `nullable` annotations (`#nullable enable`). Avoid returning null collections—return empty collections (`Array.Empty<T>()` or `Enumerable.Empty<T>()`) instead.
- **Data Records & Models**: Use `record` or `readonly struct` for immutable Data Transfer Objects (DTOs) and API contracts.
- **Pattern Matching**: Utilize modern C# pattern matching features (`switch` expressions, property patterns) to streamline conditionals.

## 4. Error Handling & API Responses
- **Global Exception Handling**: Prefer central exception handling middleware or standard `.NET` Problem Details (`IProblemDetailsService`) over repetitive try-catch blocks inside individual API endpoints.
- **Standard HTTP Responses**: Always return strongly typed `ActionResult<T>` or `IResult` with proper HTTP status codes (e.g., `200 OK`, `201 Created`, `400 BadRequest`, `404 NotFound`).

## 5. Naming Conventions & Code Style
- Follow official C# / .NET conventions:
  - Use **PascalCase** for Classes, Interfaces, Enums, Methods, Properties, and Public Fields.
  - Use **camelCase** prefixed with an underscore (`_`) for private read-only fields (e.g., `private readonly IUserService _userService;`).
  - Prefix Interface names with `I` (e.g., `IUserRepository`).
  - Name async methods with an `Async` suffix (e.g., `GetUserByIdAsync`).