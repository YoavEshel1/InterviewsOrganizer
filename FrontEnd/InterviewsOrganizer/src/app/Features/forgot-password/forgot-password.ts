import { ChangeDetectionStrategy, Component, inject, signal, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { ErrorStateMatcher } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DomSanitizer } from '@angular/platform-browser';
import { RouterLink } from '@angular/router';
import { email, form, FormField, required, submit } from '@angular/forms/signals';
import { firstValueFrom } from 'rxjs';
import { NgxTurnstileComponent } from 'ngx-turnstile';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/auth/auth.service';
import { getServerErrorMessage } from '../../core/auth/server-error';
import { LOGIN_ICONS } from '../login/login-form/login-icons';

@Component({
  selector: 'app-forgot-password',
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
  templateUrl: './forgot-password.html',
  styleUrls: ['../login/login-form/login-form.scss', './forgot-password.scss'],
})
export class ForgotPassword {
  private readonly authService = inject(AuthService);
  private readonly turnstile = viewChild(NgxTurnstileComponent);

  protected readonly siteKey = environment.turnstile.siteKey;
  protected readonly turnstileToken = signal<string | null>(null);
  protected readonly isSubmitted = signal(false);
  protected readonly errorMessage = signal('');

  protected readonly forgotModel = signal({ email: '' });

  protected readonly forgotForm = form(this.forgotModel, (path) => {
    required(path.email, { message: 'Enter your email address' });
    email(path.email, { message: 'Enter a valid email address' });
  });

  protected readonly emailMatcher: ErrorStateMatcher = {
    isErrorState: () => this.forgotForm.email().touched() && this.forgotForm.email().invalid(),
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

    void submit(this.forgotForm, async () => {
      const turnstileToken = this.turnstileToken();
      if (!turnstileToken) {
        this.errorMessage.set('Please complete the human verification.');
        return undefined;
      }

      this.errorMessage.set('');
      try {
        await firstValueFrom(
          this.authService.forgotPassword({ email: this.forgotModel().email, turnstileToken }),
        );
        this.isSubmitted.set(true);
      } catch (error) {
        // Turnstile tokens are single-use, so a new challenge is needed for the next attempt.
        this.turnstileToken.set(null);
        this.turnstile()?.reset();
        this.errorMessage.set(
          getServerErrorMessage(error, 'Something went wrong. Please try again.'),
        );
      }
      return undefined;
    });
  }
}
