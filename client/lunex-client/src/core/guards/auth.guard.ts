import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../services';
import { ToastService } from '../services';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  const toastService = inject(ToastService);

  if (accountService.isAuthenticated()) {
    return true;
  }

  toastService.warning('Authentication Required', 'Please log in to access this page');
  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};

export const guestGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  if (!accountService.isAuthenticated()) {
    return true;
  }

  router.navigate(['/members']);
  return false;
};