import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AccountService } from '../services';
import { ToastService } from '../services';

export const authInterceptor: HttpInterceptorFn = (request: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const accountService = inject(AccountService);
  const toastService = inject(ToastService);

  const token = accountService.getToken();

  if (token) {
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !request.url.includes('/login') && !request.url.includes('/register')) {
        accountService.logout();
        toastService.warning('Session Expired', 'Please log in again');
        return throwError(() => new Error('Unauthorized'));
      }

      const errorMessage = getErrorMessage(error);
      toastService.error('Error', errorMessage);
      return throwError(() => error);
    })
  );
};

function getErrorMessage(error: HttpErrorResponse): string {
  if (error.error instanceof ErrorEvent) {
    return error.error.message;
  }

  if (typeof error.error === 'string') {
    return error.error;
  }

  if (error.error?.message) {
    return error.error.message;
  }

  if (error.error?.errors) {
    const validationErrors = Object.values(error.error.errors).flat();
    return validationErrors.join(', ');
  }

  return `Error ${error.status}: ${error.statusText}`;
}