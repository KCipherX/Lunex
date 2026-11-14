import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { InputComponent } from '../../../shared/components/input/input.component';
import { AuthLayoutComponent } from '../auth-layout/auth-layout';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, ButtonComponent, InputComponent, AuthLayoutComponent],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css',
})
export class ForgotPasswordComponent {
  private fb = inject(FormBuilder);
  loading = signal(false);
  submitted = signal(false);
  emailSent = signal(false);

  forgotForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
  });

  get emailError(): string {
    if (!this.submitted()) return '';
    const control = this.forgotForm.get('email');
    if (control?.hasError('required')) return 'Email is required';
    if (control?.hasError('email')) return 'Please enter a valid email';
    return '';
  }

  async onSubmit() {
    this.submitted.set(true);
    
    if (this.forgotForm.invalid) {
      this.forgotForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    try {
      await new Promise(resolve => setTimeout(resolve, 1500));
      this.emailSent.set(true);
    } finally {
      this.loading.set(false);
    }
  }

  resendEmail() {
    this.emailSent.set(false);
    this.submitted.set(false);
  }
}