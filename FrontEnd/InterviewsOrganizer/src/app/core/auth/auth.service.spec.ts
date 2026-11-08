import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AuthService } from './auth.service';

describe('AuthService password reset', () => {
  let service: AuthService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(AuthService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('posts forgotPassword to /api/auth/forgot-password', () => {
    service.forgotPassword({ email: 'a@b.com', turnstileToken: 'tok' }).subscribe();

    const req = http.expectOne('/api/auth/forgot-password');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'a@b.com', turnstileToken: 'tok' });
    req.flush(null);
  });

  it('posts resetPassword to /api/auth/reset-password', () => {
    const body = { email: 'a@b.com', token: 't', newPassword: 'Abcdef1!', turnstileToken: 'tok' };
    service.resetPassword(body).subscribe();

    const req = http.expectOne('/api/auth/reset-password');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush(null);
  });
});
