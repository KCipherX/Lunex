import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AccountService } from '../../../core/services/account-service';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { InputComponent } from '../../../shared/components/input/input.component';
import { AuthLayoutComponent } from '../auth-layout/auth-layout';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, ButtonComponent, InputComponent, AuthLayoutComponent],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);

  loading = signal(false);
  showPassword = signal(false);
  submitted = signal(false);

  loginForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(1)]],
    rememberMe: [false],
  });

  get emailError(): string {
    if (!this.submitted()) return '';

    const control = this.loginForm.get('email');
    if (control?.hasError('required')) return 'Email is required';
    if (control?.hasError('email')) return 'Please enter a valid email';
    return '';
  }

  get passwordError(): string {
    if (!this.submitted()) return '';

    const control = this.loginForm.get('password');
    if (control?.hasError('required')) return 'Password is required';
    return '';
  }

  async onSubmit() {
    this.submitted.set(true);

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    try {
      const { email, password } = this.loginForm.getRawValue();
      await firstValueFrom(this.accountService.login({ email, password }));
    } finally {
      this.loading.set(false);
    }
  }

}