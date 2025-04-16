import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router } from "@angular/router";
import { AuthService } from "./auth.service";

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const requiredAuth = route.data["requiresAuth"] ?? true;
    const isLoggedIn = this.authService.isLoggedIn();

    if (requiredAuth && !isLoggedIn) {
      this.router.navigate(["/login"]);
      return false;
    }

    if (!requiredAuth && isLoggedIn) {
      this.router.navigate(["/dashboard"]);
      return false;
    }
    return true;
  }
}
