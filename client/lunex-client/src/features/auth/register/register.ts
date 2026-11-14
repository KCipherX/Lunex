import { Component, inject, signal, computed } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AccountService } from '../../../core/services/account-service';
import { RegisterCreds } from '../../../types/User';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { InputComponent } from '../../../shared/components/input/input.component';
import { AuthLayoutComponent } from '../auth-layout/auth-layout';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, ButtonComponent, InputComponent, AuthLayoutComponent],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);

  loading = signal(false);
  showPassword = signal(false);
  showConfirmPassword = signal(false);
  submitted = signal(false);
  maxDate = computed(() => {
    const today = new Date();
    today.setMinutes(today.getMinutes() - today.getTimezoneOffset());
    return today.toISOString().slice(0, 10);
  });

  registerForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(100)]],
    confirmPassword: ['', [Validators.required]],
    gender: ['', [Validators.required]],
    city: ['', [Validators.required, Validators.maxLength(100)]],
    country: ['', [Validators.required, Validators.maxLength(100)]],
    dateOfBirth: ['', [Validators.required]],
    description: ['', [Validators.maxLength(500)]],
    terms: [false, [Validators.requiredTrue]],
  });

  get nameError(): string {
    if (!this.submitted()) return '';
    const control = this.registerForm.get('name');
    if (control?.hasError('required')) return 'Name is required';
    if (control?.hasError('minlength')) return 'Name must be at least 2 characters';
    if (control?.hasError('maxlength')) return 'Name must be less than 100 characters';
    return '';
  }

  get emailError(): string {
    if (!this.submitted()) return '';
    const control = this.registerForm.get('email');
    if (control?.hasError('required')) return 'Email is required';
    if (control?.hasError('email')) return 'Please enter a valid email';
    return '';
  }

  get passwordError(): string {
    if (!this.submitted()) return '';
    const control = this.registerForm.get('password');
    if (control?.hasError('required')) return 'Password is required';
    if (control?.hasError('minlength')) return 'Password must be at least 8 characters';
    if (control?.hasError('maxlength')) return 'Password must be less than 100 characters';
    return '';
  }

  get confirmPasswordError(): string {
    if (!this.submitted()) return '';
    const control = this.registerForm.get('confirmPassword');
    const password = this.registerForm.get('password')?.value;
    if (control?.hasError('required')) return 'Please confirm your password';
    if (control?.value !== password) return 'Passwords do not match';
    return '';
  }

  get genderError(): string {
    if (!this.submitted()) return '';
    const control = this.registerForm.get('gender');
    if (control?.hasError('required')) return 'Please select your gender';
    return '';
  }

  get cityError(): string {
    if (!this.submitted()) return '';
    const control = this.registerForm.get('city');
    if (control?.hasError('required')) return 'City is required';
    if (control?.hasError('maxlength')) return 'City must be less than 100 characters';
    return '';
  }

  get countryError(): string {
    if (!this.submitted()) return '';
    const control = this.registerForm.get('country');
    if (control?.hasError('required')) return 'Country is required';
    if (control?.hasError('maxlength')) return 'Country must be less than 100 characters';
    return '';
  }

  get dateOfBirthError(): string {
    if (!this.submitted()) return '';
    const control = this.registerForm.get('dateOfBirth');
    if (control?.hasError('required')) return 'Date of birth is required';
    return '';
  }

  get descriptionError(): string {
    if (!this.submitted()) return '';
    const control = this.registerForm.get('description');
    if (control?.hasError('maxlength')) return 'Description must be less than 500 characters';
    return '';
  }

  get termsError(): string {
    if (!this.submitted()) return '';
    const control = this.registerForm.get('terms');
    if (control?.hasError('required')) return 'You must accept the terms and conditions';
    return '';
  }

  async onSubmit() {
    this.submitted.set(true);
    
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const formValue = this.registerForm.getRawValue();
    if (formValue.confirmPassword !== formValue.password) {
      return;
    }

    this.loading.set(true);
    const creds: RegisterCreds = {
      name: formValue.name,
      email: formValue.email,
      password: formValue.password,
      gender: formValue.gender,
      city: formValue.city,
      country: formValue.country,
      dateOfBirth: formValue.dateOfBirth,
      description: formValue.description,
    };

    try {
      await firstValueFrom(this.accountService.register(creds));
    } finally {
      this.loading.set(false);
    }
  }

  togglePassword() {
    this.showPassword.update((v: boolean) => !v);
  }

  toggleConfirmPassword() {
    this.showConfirmPassword.update((v: boolean) => !v);
  }
}