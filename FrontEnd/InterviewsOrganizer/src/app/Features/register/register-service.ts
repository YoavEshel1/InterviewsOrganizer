import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { RegisterData} from './models/registerData';
import { RegisterResponse } from './models/registerResponse';

@Injectable({
  providedIn: 'root',
})
export class RegisterService {
  private readonly http = inject(HttpClient);

  register(registerData: RegisterData, turnstileToken: string): Observable<RegisterResponse> {
    if (!this.validatePasswordMatch(registerData)) {
      return throwError(() => new Error('Passwords do not match'));
    }
    const { confirmPassword, ...fields } = registerData;
    return this.http.post<RegisterResponse>(
      '/api/auth/register',
      { ...fields, turnstileToken },
      { withCredentials: true },
    );
  }

  validatePasswordMatch(registerData: RegisterData): boolean {
    return registerData.confirmPassword === registerData.password;
  }
}
