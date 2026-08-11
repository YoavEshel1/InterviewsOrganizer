import { Component } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { LoginService } from '../login-service';

@Component({
  selector: 'app-login-form',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
  ],
  templateUrl: './login-form.html',
  styleUrl: './login-form.scss',
})
export class LoginForm {
  form: FormGroup;
  hidePassword = true;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private loginService: LoginService,
    private router: Router,
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  get email() { return this.form.get('email')!; }
  get password() { return this.form.get('password')!; }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.errorMessage = '';
    const { email, password } = this.form.value;
    this.loginService.login(email, password).subscribe({
      next: () => this.router.navigate(['/companies']),
      error: () => (this.errorMessage = 'Invalid email or password.'),
    });
  }
}
