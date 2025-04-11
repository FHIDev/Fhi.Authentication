import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor() {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          const location = error.headers.get('Location');
          if (location) {
            window.location.href = location;
          } else {
            window.location.href = '/error';
          }
          const returnUrl = window.location.origin + window.location.pathname + window.location.search;
          //const returnUrl = window.location.pathname + window.location.search;

          window.location.href = "/login?ReturnUrl=" + returnUrl;
        }
        return throwError(error);
      })
    );
  }
}
