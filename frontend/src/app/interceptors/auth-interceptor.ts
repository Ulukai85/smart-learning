import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth-service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  if (authService.isLoggedIn()) {
    const newReq = req.clone({
      headers: req.headers.append('Authorization', 'Bearer ' + authService.getToken()),
    });
    return next(newReq);
  } else {
    return next(req);
  }
};
