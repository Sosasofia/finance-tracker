import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { jwtDecode } from "jwt-decode";
import { BehaviorSubject, Observable } from "rxjs";
import { tap } from "rxjs/operators";

import { environment } from "../../../environments/environment";
import { AuthResponse } from "../../models/auth-response.model";
import { UserCredentials } from "../../models/user-credentials.model";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private readonly apiUrl = environment.apiUrl + "/auth";
  private readonly TOKEN_KEY = "auth_token";

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(
    this.hasToken()
  );

  isAuthenticated$: Observable<boolean> = this.isAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) { }

  login(credentials: UserCredentials): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/login`, credentials, {
        headers: {
          "Content-Type": "application/json",
        },
      })
      .pipe(
        tap((response) => {
          this.setToken(response.token);
          this.isAuthenticatedSubject.next(true);
        }),
      );
  }

  logout() {
    this.removeToken();
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(["/login"]);
  }

  getToken(): string | null {
    return this.getTokenFromStorage();
  }

  isLoggedIn(): boolean {
    return this.hasToken();
  }

  get isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  // Private helpers
  private hasToken(): boolean {
    const token = this.getTokenFromStorage();
    return !!token && !this.isTokenExpired(token);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  private removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  private getTokenFromStorage(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private isTokenExpired(token: string): boolean {
    const decode: any = jwtDecode(token);

    const now = Math.floor(Date.now() / 1000);
    return decode.exp < now;
  }
}
