import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { constants } from '../../shared/constants';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const token = localStorage.getItem(constants.TOKEN_KEY) || sessionStorage.getItem(constants.TOKEN_KEY);

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        localStorage.removeItem(constants.TOKEN_KEY);
        sessionStorage.removeItem(constants.TOKEN_KEY);
        localStorage.removeItem(constants.USER_KEY);
        router.navigate(['/login']);
      }

      return throwError(() => error);
    })
  );
};
