import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { RegisterService } from './register-service';

describe('RegisterService', () => {
  let service: RegisterService;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(RegisterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('posts the payload with the Turnstile token and without confirmPassword', () => {
    const http = TestBed.inject(HttpTestingController);
    service
      .register(
        { email: 'a@b.com', password: 'Abcdef1!', confirmPassword: 'Abcdef1!', firstName: 'A', lastName: 'B' },
        'tok',
      )
      .subscribe();

    const req = http.expectOne('/api/auth/register');
    expect(req.request.method).toBe('POST');
    expect(req.request.withCredentials).toBe(true);
    expect(req.request.body).toEqual({
      email: 'a@b.com',
      password: 'Abcdef1!',
      firstName: 'A',
      lastName: 'B',
      turnstileToken: 'tok',
    });
    http.verify();
  });
});
