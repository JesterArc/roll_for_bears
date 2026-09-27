import {HttpInterceptorFn} from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthenticationService } from './AuthenticationService';

export const Interceptor: HttpInterceptorFn = (req, next) => {
  const _authenticationService = inject(AuthenticationService);
  const accessToken = _authenticationService.getAccessToken();

  if (!accessToken)
    return next(req);

  const request = req.clone({
    setHeaders: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  return next(request);
}
