import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-login-area',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './login-area.html',
  styleUrl: './login-area.scss',
})
export class LoginArea {
  private router = inject(Router);

  login() {    
    this.router.navigate(['/login']);
  }
  signUp() {
    this.router.navigate(['/signup']);
  }

}


