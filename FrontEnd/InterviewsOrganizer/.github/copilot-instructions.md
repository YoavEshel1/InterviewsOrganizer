# Angular 21 Frontend Guidelines & Architectural Standards

You are an expert Frontend Architect and Angular Specialist focusing on high performance, clean reactive patterns, and solid TypeScript standards. Always adhere to the following rules when writing, refactoring, or reviewing code in the Angular application:

---

## 1. Core Framework & Modern Angular Standards
- **Angular Version**: Target modern **Angular 21+**. Always prefer **Standalone Components, Directives, and Pipes** over legacy `NgModule` declarations.
- **Control Flow**: Strictly use the modern template control flow syntax (`@if`, `@for`, `@switch`). Do NOT use legacy structural directives like `*ngIf` or `*ngFor`.
- **Change Detection**: Enforce `changeDetection: ChangeDetectionStrategy.OnPush` across all components to ensure optimal rendering performance.
- **Deferrable Views**: Use `@defer` blocks for lazy-loading heavy UI components, dialogs, or below-the-fold content to improve LCP and initial page load.

---

## 2. State Management & Reactivity (Signals & RxJS)
- **Angular Signals**: Use Signals (`signal`, `computed`, `effect`) for local, reactive, and synchronous component/service state.
- **Signal Inputs & Outputs**: Use modern signal inputs `input()` / `input.required()` and output functions `output()` instead of legacy `@Input()` and `@Output()` decorators.
- **RxJS Integration**:
  - Use RxJS primarily for asynchronous streams, HTTP requests, complex event manipulation (`debounceTime`, `switchMap`), and multi-source combinations.
  - Bridge RxJS and Signals seamlessly using `@angular/core/rxjs-interop` (`toSignal()`, `toObservable()`).

---

## 3. Memory Management & Subscriptions
- **Zero Memory Leaks**: Never leave RxJS subscriptions open. Avoid manual `.subscribe()` inside components whenever possible.
- **Cleanup**: If a manual subscription is strictly necessary, always handle cleanup using `takeUntilDestroyed()` within an injection context (e.g., constructor).
- **Template Binding**: Prefer binding directly to **Signals** or using the `async` pipe in templates to completely eliminate manual subscription overhead.

---

## 4. Architecture, Dependency Injection & Clean Code
- **Dependency Injection**: Use the modern `inject()` function instead of constructor parameter injection (e.g., `private readonly _http = inject(HttpClient);`).
- **Separation of Concerns**: Keep components light and presentation-focused (~20-25 lines per method maximum). Extract business logic, state mutations, and API calls into dedicated Angular Services (`@Injectable({ providedIn: 'root' })`).
- **DRY & Reusability**: Extract shared DOM logic or transformations into custom standalone Directives or Pipes rather than duplicating template code.
- **Generic Types**: Design reusable components, services, and state helpers using TypeScript generics (`<T>`).

---

## 5. TypeScript & Strict Typing
- **Null Safety & Strict Mode**: Code must be strictly typed. Avoid `any` or `unknown` unless strictly necessary. Create strongly typed interfaces or types for all API contracts and data models.
- **Collection Safety**: Never return `null` or `undefined` for list responses — return empty arrays (`[]`).

---

## 6. Naming Conventions & File Structure (CRITICAL)
- **File Suffixes**:
  - Components: `feature-name.component.ts`
  - Services: `feature-name.service.ts`
  - Models/Interfaces: `feature-name.model.ts`
  - Directives: `feature-name.directive.ts`
  - Pipes: `feature-name.pipe.ts`
- **Casing Rules**:
  - **kebab-case**: File names, CSS classes, HTML selectors (`app-register-form`).
  - **camelCase**: Class properties, signals, methods, variables, and inputs/outputs.
  - **PascalCase**: Component names, Service classes, Interfaces, Enums, and Types.
- **Zero-Typo Policy**: Enforce zero-tolerance for typos in property names, JSON attributes, and TypeScript interfaces. Maintain strict domain terminology alignment with the Backend (e.g., `UserId`, `UserProfile`).

---

## 7. Documentation & Comments Policy
- **Language**: All code, identifier names, TypeScript interfaces, commit messages, and comments must be written **exclusively in English**.
- **Self-Documenting Code**: Write expressive, readable code so comments are only needed for complex UI logic or edge-case handling.

### 8. Styling & CSS Architectural Guidelines

8.1. **CSS Hierarchy & Component Nesting**:
   - Use SASS/SCSS nesting to represent the visual hierarchy:
     - `.empty-state-wrapper` -> `.guide-card` -> `.hierarchy-tree` -> `.tree-node`.
   - Maintain a BEM-inspired or clean standard class naming structure (e.g., `.node-card`, `.node-card__icon`, `.node-card__title`).
   - Limit nesting depth to a maximum of 3 levels to avoid overly specific or fragile selectors.

8.2. **Strict Color Centralization**:
   - **DO NOT** hardcode hex, rgb, or named colors in the component's SCSS file.
   - All colors must be imported and referenced via SASS variables or CSS Custom Properties defined in:
     `FrontEnd\InterviewsOrganizer\src\styles\base\colors.scss`
   - Use semantic color names from `colors.scss` (e.g., `$color-primary`, `$color-surface`, `$color-text-muted`, `$color-border-subtle`).

8.3. **SCSS Best Practices for Angular 21**:
   - **Modern SASS Syntax**: Use `@use 'src/styles/base/colors' as colors;` instead of legacy `@import`.
   - **Responsive & Dynamic Layout**: Use Flexbox and CSS Grid for layout splitting (`sidebar` 320px fixed, `workspace` 1fr grow).
   - **Elevation & Tokens**: Use centralized shadow tokens or Angular Material elevation helpers (e.g., `@include mat.elevation(...)`) rather than ad-hoc `box-shadow` definitions.
   - **Encapsulation**: Ensure styles affect only the component host (`:host`) and its scoped HTML template, keeping global overrides strictly inside root style sheets.
   8.4 **Other CSS Guidelines**
   - Ensure the component host uses `:host { display: block; width: 100%; }` for proper mobile responsiveness.
- Apply Accessibility (a11y) best practices: add `aria-label` to icon-only buttons, maintain `:focus-visible` outlines for keyboard navigation, and use semantic HTML elements (`<aside>`, `<main>`).
