import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An unexpected error occurred.';

        if (error.status === 429) {
          errorMessage = 'Too many login attempts. Please try again later.';
        } else if (error.status === 500 || error.status === 0) {
          errorMessage =
            'The server is temporarily unavailable. Please try again in a few minutes.';
        } else {
          errorMessage = error.error?.detail || 'Request failed.';
        }

        return throwError(() => ({ ...error, message: errorMessage }));
      })
    );
  }
}
