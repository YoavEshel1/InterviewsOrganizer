import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthService } from '../../core/auth/auth.service';

@Injectable() //only for login components
export class LoginService {
  private readonly authService = inject(AuthService);

  login(email: string, password: string): Observable<void> {
    return this.authService.login(email, password).pipe(map(() => undefined));
  }
}
