import { HttpErrorResponse } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { throwError, of } from 'rxjs';
import { vi } from 'vitest';
import { Register } from './register';
import { RegisterService } from './register-service';

describe('Register', () => {
  const registerService = { register: vi.fn() };
  const router = { navigate: vi.fn().mockResolvedValue(true) };

  function createComponent(): Register {
    TestBed.configureTestingModule({
      imports: [Register],
      providers: [
        { provide: RegisterService, useValue: registerService },
        { provide: Router, useValue: router },
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

    component['onSubmit'](new Event('submit'));
    await new Promise((resolve) => setTimeout(resolve));

    const errors = component['registerForm'].confirmPassword().errors();
    expect(errors[0].message).toBe("Passwords must have at least one lowercase ('a'-'z').");
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('redirects to the login page on success', async () => {
    registerService.register.mockReturnValue(of({}));
    const component = createComponent();
    fill(component, 'Abcdef1!');

    component['onSubmit'](new Event('submit'));
    await new Promise((resolve) => setTimeout(resolve));

    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });
});
