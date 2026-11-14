import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { LoginCreds, RegisterCreds, User } from '../../types/User';
import { tap, catchError, of } from 'rxjs';
import { ToastService } from './toast.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private toastService = inject(ToastService);

  private readonly baseUrl = `${environment.apiUrl}/account`;

  public currentUser = signal<User | null>(null);
  public isAuthenticated = computed(() => !!this.currentUser());

  constructor() {
    this.loadStoredUser();
  }

  private loadStoredUser() {
    const stored = localStorage.getItem('user');
    if (stored) {
      try {
        this.currentUser.set(JSON.parse(stored));
      } catch {
        this.clearStorage();
      }
    }
  }

  register(creds: RegisterCreds) {
    return this.http.post<User>(`${this.baseUrl}/register`, creds).pipe(
      tap((user) => this.handleAuthSuccess(user)),
      catchError((error) => {
        this.toastService.error('Registration Failed', error.error?.message || 'Please try again');
        return of(null);
      })
    );
  }

  login(creds: LoginCreds) {
    return this.http.post<User>(`${this.baseUrl}/login`, creds).pipe(
      tap((user) => this.handleAuthSuccess(user)),
      catchError((error) => {
        this.toastService.error('Login Failed', error.error?.message || 'Invalid credentials');
        return of(null);
      })
    );
  }

  private handleAuthSuccess(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
    this.toastService.success('Welcome!', `Hello, ${user.name}`);
    this.router.navigate(['/members']);
  }

  logout() {
    this.clearStorage();
    this.currentUser.set(null);
    this.router.navigate(['/']);
    this.toastService.info('Logged Out', 'You have been logged out successfully');
  }

  getToken(): string | null {
    return this.currentUser()?.token ?? null;
  }

  private clearStorage() {
    localStorage.removeItem('user');
  }

  updateProfile(user: Partial<User>) {
    const current = this.currentUser();
    if (current) {
      const updated = { ...current, ...user };
      localStorage.setItem('user', JSON.stringify(updated));
      this.currentUser.set(updated);
    }
  }
}