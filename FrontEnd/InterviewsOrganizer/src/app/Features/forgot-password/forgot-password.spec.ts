import { HttpErrorResponse } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { vi } from 'vitest';
import { AuthService } from '../../core/auth/auth.service';
import { ForgotPassword } from './forgot-password';

describe('ForgotPassword', () => {
  const authService = { forgotPassword: vi.fn() };

  function createComponent() {
    TestBed.configureTestingModule({
      imports: [ForgotPassword],
      providers: [provideRouter([]), { provide: AuthService, useValue: authService }],
    });
    const fixture = TestBed.createComponent(ForgotPassword);
    fixture.detectChanges();
    return fixture;
  }

  async function submit(component: ForgotPassword): Promise<void> {
    component['onSubmit'](new Event('submit'));
    await new Promise((resolve) => setTimeout(resolve));
  }

  beforeEach(() => authService.forgotPassword.mockReset());

  it('disables the submit button until a Turnstile token is present', () => {
    const fixture = createComponent();
    const button: HTMLButtonElement = fixture.nativeElement.querySelector('button[type="submit"]');
    expect(button.disabled).toBe(true);

    fixture.componentInstance['turnstileToken'].set('tok');
    fixture.detectChanges();
    expect(button.disabled).toBe(false);
  });

  it('does not call the API without a token', async () => {
    const component = createComponent().componentInstance;
    component['forgotModel'].set({ email: 'a@b.com' });

    await submit(component);

    expect(authService.forgotPassword).not.toHaveBeenCalled();
  });

  it('shows the enumeration-safe confirmation after success', async () => {
    authService.forgotPassword.mockReturnValue(of(undefined));
    const fixture = createComponent();
    const component = fixture.componentInstance;
    component['forgotModel'].set({ email: 'a@b.com' });
    component['turnstileToken'].set('tok');

    await submit(component);
    fixture.detectChanges();

    expect(authService.forgotPassword).toHaveBeenCalledWith({ email: 'a@b.com', turnstileToken: 'tok' });
    expect(component['isSubmitted']()).toBe(true);
    expect(fixture.nativeElement.textContent).toContain(
      'If an account exists with this email, a password reset link has been sent.',
    );
  });

  it('surfaces a Turnstile failure and clears the token', async () => {
    authService.forgotPassword.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 400, error: 'Turnstile verification failed.' })),
    );
    const component = createComponent().componentInstance;
    component['forgotModel'].set({ email: 'a@b.com' });
    component['turnstileToken'].set('tok');

    await submit(component);

    expect(component['errorMessage']()).toBe('Turnstile verification failed.');
    expect(component['isSubmitted']()).toBe(false);
    expect(component['turnstileToken']()).toBeNull();
  });
});
