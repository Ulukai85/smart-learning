import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth-service';
import { ToastService } from '../services/toast-service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const toast = inject(ToastService);
  const router = inject(Router);

  const token = auth.getToken();

  const authReq = token
    ? req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`),
      })
    : req;

  return next(authReq).pipe(
    catchError((err) => {
      console.log('Error:', err);
      if (err.status == 401 && auth.isLoggedIn()) {
        const firstTime = auth.handleSessionExpired();

        if (firstTime) {
          toast.info('Please login again', 'Session expired!');
          if (router.url !== '/login') router.navigateByUrl('/login');
        }
      }
      return throwError(() => err);
    }),
  );
};
