import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';

import { UserCredentials } from '../../models/user-credentials.model';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService, { provide: Router, useValue: spy }],
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should log in and store token', () => {
    const credentials: UserCredentials = {
      username: 'test@example.com',
      password: '123456',
    };
    const fakeToken = 'fake.jwt.token';
    const fakeResponse = { token: fakeToken };

    service.login(credentials).subscribe((response) => {
      expect(response.token).toBe(fakeToken);
      expect(service.getToken()).toBe(fakeToken);
      expect(service.isAuthenticated).toBeTrue();
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/login`);
    expect(req.request.method).toBe('POST');
    req.flush(fakeResponse);
  });

  it('should logout and remove token', () => {
    localStorage.setItem('auth_token', 'dummy');
    service.logout();
    expect(localStorage.getItem('auth_token')).toBeNull();
    expect(service.isAuthenticated).toBeFalse();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should detect expired token', () => {
    const expiredPayload = {
      exp: Math.floor(Date.now() / 1000) - 3600,
    };
    const token = createFakeToken(expiredPayload);
    localStorage.setItem('auth_token', token);
    expect(service.isLoggedIn()).toBeFalse();
  });

  it('should detect valid token', () => {
    const validPayload = {
      exp: Math.floor(Date.now() / 1000) + 3600,
    };
    const token = createFakeToken(validPayload);
    localStorage.setItem('auth_token', token);
    expect(service.isLoggedIn()).toBeTrue();
  });
});

function createFakeToken(payload: any): string {
  const base64 = (obj: any) => btoa(JSON.stringify(obj)).replace(/=/g, '');
  return `${base64({ alg: 'HS256', typ: 'JWT' })}.${base64(payload)}.signature`;
}
