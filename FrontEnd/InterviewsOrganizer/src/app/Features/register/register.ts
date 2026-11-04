import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { ErrorStateMatcher } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DomSanitizer } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { email, form, FormField, minLength, required, submit, validate } from '@angular/forms/signals';
import type { FieldTree } from '@angular/forms/signals';
import { firstValueFrom } from 'rxjs';
import { LOGIN_ICONS } from '../login/login-form/login-icons';
import { RegisterData } from './models/registerData';
import { RegisterService } from './register-service';

/** Mirrors the server's Identity password rules so users get feedback before submitting. */
const PASSWORD_RULES = [
  {
    kind: 'nonAlphanumeric',
    test: /[^a-zA-Z0-9]/,
    message: 'Password must have at least one non-alphanumeric character.',
  },
  { kind: 'digit', test: /[0-9]/, message: "Password must have at least one digit ('0'-'9')." },
  { kind: 'uppercase', test: /[A-Z]/, message: "Password must have at least one uppercase letter ('A'-'Z')." },
] as const;

@Component({
  selector: 'app-register',
  changeDetection: ChangeDetectionStrategy.Default,
  imports: [
    FormField,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './register.html',
  styleUrls: ['../login/login-form/login-form.scss', './register.scss'],
})
export class Register {
  private readonly registerService = inject(RegisterService);
  private readonly router = inject(Router);

  protected readonly hidePassword = signal(true);

  protected readonly registerModel = signal<RegisterData>({
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
  });

  protected readonly registerForm = form(this.registerModel, (path) => {
    required(path.firstName, { message: 'Enter your first name' });
    required(path.lastName, { message: 'Enter your last name' });
    required(path.email, { message: 'Enter your email address' });
    email(path.email, { message: 'Enter a valid email address' });
    required(path.password, { message: 'Enter a password' });
    minLength(path.password, 6, { message: 'Password must be at least 6 characters' });
    for (const rule of PASSWORD_RULES) {
      validate(path.password, ({ value }) =>
        value() && !rule.test.test(value()) ? { kind: rule.kind, message: rule.message } : undefined,
      );
    }
    required(path.confirmPassword, { message: 'Confirm your password' });
    validate(path.confirmPassword, ({ value, valueOf }) =>
      value() && value() !== valueOf(path.password)
        ? { kind: 'mismatch', message: 'Passwords do not match' }
        : undefined,
    );
  });

  constructor() {
    const registry = inject(MatIconRegistry);
    const sanitizer = inject(DomSanitizer);
    for (const [name, markup] of Object.entries(LOGIN_ICONS)) {
      registry.addSvgIconLiteral(name, sanitizer.bypassSecurityTrustHtml(markup));
    }
  }

  /** mat-form-field only knows reactive forms, so signal-form state is bridged through a matcher. */
  private matcherFor(field: FieldTree<string>): ErrorStateMatcher {
    return { isErrorState: () => field().touched() && field().invalid() };
  }

  protected readonly matchers = {
    firstName: this.matcherFor(this.registerForm.firstName),
    lastName: this.matcherFor(this.registerForm.lastName),
    email: this.matcherFor(this.registerForm.email),
    password: this.matcherFor(this.registerForm.password),
    confirmPassword: this.matcherFor(this.registerForm.confirmPassword),
  };
  protected onSubmit(event: Event): void {
    // Prevent reloading the page on form submission
    event.preventDefault();

    // submit() touches every field and only runs the action when the form is valid.
    // Errors returned from the action are attached to the given field.
    void submit(this.registerForm, async () => {
      try {
        await firstValueFrom(this.registerService.register(this.registerModel()));
        await this.router.navigate(['/login']);
        return undefined;
      } catch (error) {
        return {
          fieldTree: this.registerForm.confirmPassword,
          kind: 'server',
          message: this.getServerErrorMessage(error),
        };
      }
    });
  }

  private getServerErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 0) {
        return 'Unable to reach the server. Please try again later.';
      }
      // The API returns the error text as a plain string body.
      if (typeof error.error === 'string' && error.error.trim()) {
        return error.error;
      }
    }
    return 'Registration failed. Please try again.';
  }
}
