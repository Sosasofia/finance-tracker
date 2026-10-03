import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, of, switchMap, map } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { AuthResponse } from '../../shared/models/auth-response.model';
import { UserCredentials } from '../../shared/models/user-credentials.model';
import { UserSessionDto } from '../../shared/models/user-session-dto.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = environment.apiUrl + '/auth';

  public currentUser = signal<UserSessionDto | null>(null);
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  isAuthenticated$: Observable<boolean> = this.isAuthenticatedSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  checkSession(): Observable<UserSessionDto | null> {
    return this.http.get<UserSessionDto>(`${this.apiUrl}/me`, { withCredentials: true }).pipe(
      tap((user: UserSessionDto) => {
        this.currentUser.set(user);
        this.isAuthenticatedSubject.next(true);
      }),
      catchError(() => {
        this.currentUser.set(null);
        this.isAuthenticatedSubject.next(false);
        return of(null);
      })
    );
  }

  login(credentials: UserCredentials): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/login`, credentials, {
        withCredentials: true,
      })
      .pipe(
        switchMap((authResponse) => {
          return this.checkSession().pipe(map(() => authResponse));
        })
      );
  }

  register(credentials: any): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/register`, credentials, { withCredentials: true })
      .pipe(
        switchMap((authResponse) => {
          return this.checkSession().pipe(map(() => authResponse));
        })
      );
  }

  logout() {
    return this.http.post(`${this.apiUrl}/logout`, {}, { withCredentials: true }).pipe(
      catchError(() => {
        return of(null);
      }),
      tap(() => {
        this.currentUser.set(null);
        this.isAuthenticatedSubject.next(false);
      })
    );
  }
}
