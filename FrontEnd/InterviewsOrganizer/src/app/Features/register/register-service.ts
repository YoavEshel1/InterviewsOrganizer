import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { RegisterData } from './models/registerData';

@Injectable({
  providedIn: 'root',
})
export class RegisterService {
  private readonly http = inject(HttpClient);

  register(registerData: RegisterData): Observable<unknown> {
    if (!this.validatePasswordMatch(registerData)) {
      return throwError(() => new Error('Passwords do not match'));
    }
    const { confirmPassword, ...payload } = registerData;
    return this.http.post('/api/auth/register', payload);
  }

  validatePasswordMatch(registerData: RegisterData): boolean {
    return registerData.confirmPassword === registerData.password;
  }
}
