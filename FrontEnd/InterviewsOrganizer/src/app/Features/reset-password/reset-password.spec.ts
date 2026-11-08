import { HttpErrorResponse } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router, provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { vi } from 'vitest';
import { AuthService } from '../../core/auth/auth.service';
import { RESET_SUCCESS_MESSAGE, ResetPassword } from './reset-password';

describe('ResetPassword', () => {
  const authService = { resetPassword: vi.fn() };
  const snackBar = { open: vi.fn() };

  function createComponent(params: Record<string, string> = { email: 'a@b.com', token: 'tk+/=' }) {
    TestBed.configureTestingModule({
      imports: [ResetPassword],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authService },
        { provide: MatSnackBar, useValue: snackBar },
        { provide: ActivatedRoute, useValue: { snapshot: { queryParamMap: new Map(Object.entries(params)) } } },
      ],
    });
    const fixture = TestBed.createComponent(ResetPassword);
    fixture.detectChanges();
    return fixture;
  }

  function fill(component: ResetPassword, newPassword: string, confirmPassword = newPassword): void {
    component['resetModel'].set({ newPassword, confirmPassword });
  }

  async function submit(component: ResetPassword): Promise<void> {
    component['onSubmit'](new Event('submit'));
    await new Promise((resolve) => setTimeout(resolve));
  }

  beforeEach(() => {
    authService.resetPassword.mockReset();
    snackBar.open.mockReset();
  });

  it('shows the invalid-link state when query params are missing', () => {
    const fixture = createComponent({ email: 'a@b.com' });

    expect(fixture.nativeElement.querySelector('form')).toBeNull();
    expect(fixture.nativeElement.textContent).toContain('invalid or incomplete');
  });

  it('flags mismatched passwords', () => {
    const component = createComponent().componentInstance;
    fill(component, 'Abcdef1!', 'Different1!');

    expect(component['resetForm'].confirmPassword().errors()[0].message).toBe('Passwords do not match');
  });

  it('disables submit until the form is valid and a token exists', () => {
    const fixture = createComponent();
    const component = fixture.componentInstance;
    const button: HTMLButtonElement = fixture.nativeElement.querySelector('button[type="submit"]');

    fill(component, 'Abcdef1!');
    fixture.detectChanges();
    expect(button.disabled).toBe(true);

    component['turnstileToken'].set('tok');
    fixture.detectChanges();
    expect(button.disabled).toBe(false);
  });

  it('submits email, token, password and turnstile token, then redirects with a toast', async () => {
    authService.resetPassword.mockReturnValue(of(undefined));
    const component = createComponent().componentInstance;
    const navigate = vi.spyOn(TestBed.inject(Router), 'navigate').mockResolvedValue(true);
    fill(component, 'Abcdef1!');
    component['turnstileToken'].set('tok');

    await submit(component);

    expect(authService.resetPassword).toHaveBeenCalledWith({
      email: 'a@b.com',
      token: 'tk+/=',
      newPassword: 'Abcdef1!',
      turnstileToken: 'tok',
    });
    expect(snackBar.open).toHaveBeenCalledWith(RESET_SUCCESS_MESSAGE, 'Dismiss', expect.anything());
    expect(navigate).toHaveBeenCalledWith(['/login']);
  });

  it('shows the server error and clears the token on failure', async () => {
    authService.resetPassword.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 400, error: 'Invalid token.' })),
    );
    const component = createComponent().componentInstance;
    const navigate = vi.spyOn(TestBed.inject(Router), 'navigate');
    fill(component, 'Abcdef1!');
    component['turnstileToken'].set('tok');

    await submit(component);

    expect(component['errorMessage']()).toBe('Invalid token.');
    expect(component['turnstileToken']()).toBeNull();
    expect(navigate).not.toHaveBeenCalled();
  });
});
