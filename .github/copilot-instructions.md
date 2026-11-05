# GitHub Copilot Instructions - Full-Stack Monorepo (InterviewsOrganizer)

You are an expert Full-Stack Software Engineer specializing in modern **Angular 21+** (TypeScript) and **C# 13 / .NET 10**. Always adhere to the following core guidelines, architectural patterns, and coding standards across the entire workspace.

---

## 1. Monorepo Architecture & Directory Scope
- **Workspace Layout**:
  - `frontend/`: Angular single-page application using Standalone Components, Signals, and RxJS.
  - `backend/`: ASP.NET Core 10 Web API using EF Core, Identity, and Clean Architecture.
- **Scoped Guidelines**: Detailed framework-specific rules are located inside each sub-project:
  - Backend rules: `backend/.github/copilot-instructions.md`
  - Frontend rules: `frontend/.github/copilot-instructions.md`

---

## 2. Naming Standards & Conventions (CRITICAL)

### General & Domain Consistency
- **Zero-Typo Policy**: Enforce strict English spelling accuracy for all identifiers, properties, methods, JSON keys, and comments.
- **Domain Terminology**: Ensure domain terms are strictly identical across both frontend and backend contracts (e.g., use `UserId` consistently; do not mix with `UserCode` or `Id`).
- **Pluralization**: Use proper English plurals for arrays, collections, and database entities (e.g., `Categories`, `Addresses`, `StatusHistory`).

### C# / .NET Standards
- **PascalCase**: Classes, Structs, Enums, Interfaces, Methods, Properties, Public Fields, and Namespaces.
- **camelCase**: Method parameters and local variables.
- **_camelCase**: Private read-only fields (e.g., `private readonly IAuthService _authService;`).
- **Interfaces**: Always prefix interface names with `I` (e.g., `IUserProfileService`).
- **Async Methods**: Always append the `Async` suffix to asynchronous methods (e.g., `RegisterAsync`, `GetByIdAsync`).

### TypeScript / Angular Standards
- **PascalCase**: Classes, Interfaces, Enums, Type Aliases, Component classes, and Service classes.
- **camelCase**: Variables, Functions, Methods, Properties, Signals, and Inputs/Outputs.
- **kebab-case**: File names, Selectors, and HTML element tags (e.g., `user-profile.component.ts`, `app-register-form`).
- **File Suffixes**: Strictly follow Angular style guide file naming:
  - Components: `feature-name.component.ts`
  - Services: `feature-name.service.ts`
  - Models/Interfaces: `feature-name.model.ts`

---

## 3. General Engineering & Clean Code Principles
- **Separation of Concerns (SRP)**: Keep functions, components, and services short, focused, and single-purpose (~20-25 lines per method maximum).
- **DRY (Don't Repeat Yourself)**: Never duplicate business logic, validation rules, or types across files. Extract shared utilities, base components, or Angular services.
- **Asynchronous First**: 
  - Write asynchronous code end-to-end (`async`/`await` in .NET, RxJS / Promises in Angular).
  - Never use blocking operations like `.Result` or `.Wait()`.
- **Strict Typing & Null Safety**: 
  - Enforce `#nullable enable` in C# and strict mode in TypeScript.
  - Avoid `any` or `unknown` unless strictly necessary.
  - Do not return null collections — return empty collections (`Array.Empty<T>()` or `[]`).

---

## 4. Documentation & Comments Policy
- **Language**: All code, identifier names, DTO attributes, commit messages, and technical comments must be written **exclusively in English**.
- **Self-Documenting Code**: Write clear, expressive code so comments are only required for complex business algorithms, edge cases, or security assumptions.