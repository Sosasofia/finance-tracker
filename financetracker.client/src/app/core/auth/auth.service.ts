import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { AuthResponse } from "../../models/auth-response.model";
import { UserCredentials } from "../../models/user-credentials.model";
import { environment } from "../../../environments/environment";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private readonly apiUrl = environment.apiUrl + "/auth";
  private readonly TOKEN_KEY = "auth_token";

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(
    this.hasToken(),
  );

  constructor(private http: HttpClient) {}

  login(credentials: UserCredentials): Observable<AuthResponse> {
    console.log("API URL in production:", environment.apiUrl);
    console.log("Login credentials:", credentials);
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
    return !!this.getTokenFromStorage();
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
}
