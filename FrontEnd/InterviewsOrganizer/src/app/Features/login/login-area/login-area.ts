import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-login-area',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './login-area.html',
  styleUrl: './login-area.scss',
})
export class LoginArea {

  private router = inject(Router);
  private readonly authService = inject(AuthService);
  readonly loggedIn = this.authService.isAuthenticated;


  logout() {
    this.authService.logout().subscribe();
  }
  

  login() {
    this.router.navigate(['/login']);
  }
  
  signUp() {
    this.router.navigate(['/signup']);
  }

}


