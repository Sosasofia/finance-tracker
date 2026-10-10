import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, UrlTree } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean | UrlTree> | boolean | UrlTree {
    const requiredAuth = route.data['requiresAuth'] ?? true;

    const currentUser = this.authService.currentUser();

    if (currentUser !== null) {
      return this.handleRouting(true, requiredAuth);
    }

    return this.authService.checkSession().pipe(
      map((user) => {
        const isLoggedIn = user !== null;
        return this.handleRouting(isLoggedIn, requiredAuth);
      }),
      catchError(() => {
        return of(this.handleRouting(false, requiredAuth));
      })
    );
  }

  private handleRouting(isLoggedIn: boolean, requiredAuth: boolean): boolean {
    if (requiredAuth && !isLoggedIn) {
      this.router.navigate(['/login']);
      return false;
    }

    if (!requiredAuth && isLoggedIn) {
      this.router.navigate(['/dashboard']);
      return false;
    }

    return true;
  }
}
