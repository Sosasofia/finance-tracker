import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable()
export class CredentialsInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const isApiUrl = req.url.startsWith(environment.apiUrl) || req.url.startsWith('/api');

    if (isApiUrl) {
      const cloned = req.clone({
        withCredentials: true,
      });
      return next.handle(cloned);
    }

    return next.handle(req);
  }
}
