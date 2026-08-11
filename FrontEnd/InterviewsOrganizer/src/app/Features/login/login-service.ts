import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthService } from '../../core/auth/auth.service';

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  constructor(private authService: AuthService) {}

  login(email: string, password: string): Observable<void> {
    return this.authService.login(email, password).pipe(map(() => undefined));
  }
}
