import { Component, inject } from '@angular/core';
import { Navbar } from "../navbar/navbar";
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-header',
  imports: [Navbar],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header {
  private readonly authService = inject(AuthService);
  readonly loggedIn = this.authService.isAuthenticated;
}
