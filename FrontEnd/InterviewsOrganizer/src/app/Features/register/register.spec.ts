import { HttpErrorResponse } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { throwError, of } from 'rxjs';
import { vi } from 'vitest';
import { AuthService } from '../../core/auth/auth.service';
import { Register } from './register';
import { RegisterService } from './register-service';

describe('Register', () => {
  const registerService = { register: vi.fn() };
  const router = { navigate: vi.fn().mockResolvedValue(true) };
  const authService = { setAuthenticatedUser: vi.fn() };
  const profile = { firstName: 'Test', lastName: 'User', email: 'test@example.com', updatedAtDateTimeUtc: null };

  function createComponent(): Register {
    TestBed.configureTestingModule({
      imports: [Register],
      providers: [
        { provide: RegisterService, useValue: registerService },
        { provide: Router, useValue: router },
        { provide: AuthService, useValue: authService },
      ],
    });
    return TestBed.createComponent(Register).componentInstance;
  }

  function fill(component: Register, password: string): void {
    component['registerModel'].set({
      firstName: 'Test',
      lastName: 'User',
      email: 'test@example.com',
      password,
      confirmPassword: password,
    });
  }

  beforeEach(() => {
    registerService.register.mockReset();
    router.navigate.mockClear();
    authService.setAuthenticatedUser.mockClear();
  });

  it('should create', () => {
    expect(createComponent()).toBeTruthy();
  });

  it('reports every unmet password rule', () => {
    const component = createComponent();
    fill(component, 'abcdefg');

    const messages = component['registerForm'].password().errors().map((e) => e.message);
    expect(messages).toEqual([
      'Password must have at least one non-alphanumeric character.',
      "Password must have at least one digit ('0'-'9').",
      "Password must have at least one uppercase letter ('A'-'Z').",
    ]);
  });

  it('accepts a password that meets all rules', () => {
    const component = createComponent();
    fill(component, 'Abcdef1!');

    expect(component['registerForm'].password().errors()).toEqual([]);
  });

  it('shows the server error under the last field', async () => {
    registerService.register.mockReturnValue(
      throwError(
        () => new HttpErrorResponse({ status: 400, error: "Passwords must have at least one lowercase ('a'-'z')." }),
      ),
    );
    const component = createComponent();
    fill(component, 'ABCDEF1!');
    component['turnstileToken'].set('token-1');

    component['onSubmit'](new Event('submit'));
    await new Promise((resolve) => setTimeout(resolve));

    const errors = component['registerForm'].confirmPassword().errors();
    expect(errors[0].message).toBe("Passwords must have at least one lowercase ('a'-'z').");
    expect(router.navigate).not.toHaveBeenCalled();
    expect(component['turnstileToken']()).toBeNull();
  });

  it('signs the user in and navigates to /companies on success', async () => {
    registerService.register.mockReturnValue(of({ profile, accessToken: 'jwt' }));
    const component = createComponent();
    fill(component, 'Abcdef1!');
    component['turnstileToken'].set('token-1');

    component['onSubmit'](new Event('submit'));
    await new Promise((resolve) => setTimeout(resolve));

    expect(registerService.register).toHaveBeenCalledWith(expect.objectContaining({ email: 'test@example.com' }), 'token-1');
    expect(authService.setAuthenticatedUser).toHaveBeenCalledWith(profile, 'jwt');
    expect(router.navigate).toHaveBeenCalledWith(['/companies']);
  });

  it('does not submit before the Turnstile challenge is resolved', async () => {
    const component = createComponent();
    fill(component, 'Abcdef1!');

    component['onSubmit'](new Event('submit'));
    await new Promise((resolve) => setTimeout(resolve));

    expect(registerService.register).not.toHaveBeenCalled();
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('disables the submit button until a Turnstile token is present', () => {
    createComponent();
    const fixture = TestBed.createComponent(Register);
    const button: HTMLButtonElement = fixture.nativeElement.querySelector('button[type="submit"]');
    fixture.detectChanges();
    expect(button.disabled).toBe(true);

    fixture.componentInstance['turnstileToken'].set('token-1');
    fixture.detectChanges();
    expect(button.disabled).toBe(false);
  });
});
