import { ChangeDetectionStrategy, Component, inject, signal, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { ErrorStateMatcher } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { form, FormField, minLength, required, submit, validate } from '@angular/forms/signals';
import type { FieldTree } from '@angular/forms/signals';
import { firstValueFrom } from 'rxjs';
import { NgxTurnstileComponent } from 'ngx-turnstile';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/auth/auth.service';
import { PASSWORD_RULES } from '../../core/auth/password-rules';
import { getServerErrorMessage } from '../../core/auth/server-error';
import { LOGIN_ICONS } from '../login/login-form/login-icons';

export const RESET_SUCCESS_MESSAGE = 'Password reset successfully. Please log in with your new password.';

@Component({
  selector: 'app-reset-password',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    FormField,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    NgxTurnstileComponent,
  ],
  templateUrl: './reset-password.html',
  styleUrls: ['../login/login-form/login-form.scss', './reset-password.scss'],
})
export class ResetPassword {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);
  private readonly turnstile = viewChild(NgxTurnstileComponent);

  private readonly queryParams = inject(ActivatedRoute).snapshot.queryParamMap;
  private readonly email = this.queryParams.get('email');
  private readonly token = this.queryParams.get('token');

  protected readonly hasValidLink = !!this.email && !!this.token;
  protected readonly siteKey = environment.turnstile.siteKey;
  protected readonly turnstileToken = signal<string | null>(null);
  protected readonly hidePassword = signal(true);
  protected readonly errorMessage = signal('');

  protected readonly resetModel = signal({ newPassword: '', confirmPassword: '' });

  protected readonly resetForm = form(this.resetModel, (path) => {
    required(path.newPassword, { message: 'Enter a new password' });
    minLength(path.newPassword, 6, { message: 'Password must be at least 6 characters' });
    for (const rule of PASSWORD_RULES) {
      validate(path.newPassword, ({ value }) =>
        value() && !rule.test.test(value()) ? { kind: rule.kind, message: rule.message } : undefined,
      );
    }
    required(path.confirmPassword, { message: 'Confirm your password' });
    validate(path.confirmPassword, ({ value, valueOf }) =>
      value() && value() !== valueOf(path.newPassword)
        ? { kind: 'mismatch', message: 'Passwords do not match' }
        : undefined,
    );
  });

  /** mat-form-field only knows reactive forms, so signal-form state is bridged through a matcher. */
  private matcherFor(field: FieldTree<string>): ErrorStateMatcher {
    return { isErrorState: () => field().touched() && field().invalid() };
  }

  protected readonly matchers = {
    newPassword: this.matcherFor(this.resetForm.newPassword),
    confirmPassword: this.matcherFor(this.resetForm.confirmPassword),
  };

  constructor() {
    const registry = inject(MatIconRegistry);
    const sanitizer = inject(DomSanitizer);
    for (const [name, markup] of Object.entries(LOGIN_ICONS)) {
      registry.addSvgIconLiteral(name, sanitizer.bypassSecurityTrustHtml(markup));
    }
  }

  protected onSubmit(event: Event): void {
    event.preventDefault();

    void submit(this.resetForm, async () => {
      const turnstileToken = this.turnstileToken();
      if (!this.email || !this.token || !turnstileToken) {
        this.errorMessage.set('Please complete the human verification.');
        return undefined;
      }

      this.errorMessage.set('');
      try {
        await firstValueFrom(
          this.authService.resetPassword({
            email: this.email,
            token: this.token,
            newPassword: this.resetModel().newPassword,
            turnstileToken,
          }),
        );
        this.snackBar.open(RESET_SUCCESS_MESSAGE, 'Dismiss', { duration: 8000 });
        await this.router.navigate(['/login']);
      } catch (error) {
        // Turnstile tokens are single-use, so a new challenge is needed for the next attempt.
        this.turnstileToken.set(null);
        this.turnstile()?.reset();
        this.errorMessage.set(
          getServerErrorMessage(error, 'Password reset failed. The link may have expired.'),
        );
      }
      return undefined;
    });
  }
}
