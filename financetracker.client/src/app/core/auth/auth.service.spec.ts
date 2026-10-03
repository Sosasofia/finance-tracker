import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';

import { UserCredentials } from '../../shared/models/user-credentials.model';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';
import { UserSessionDto } from '../../shared/models/user-session-dto.model';
import { AuthResponse } from '../../shared/models/auth-response.model';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let routerSpy: jasmine.SpyObj<Router>;

  const apiUrl = environment.apiUrl + '/auth';

  beforeEach(async () => {
    (window as any).google = {
      accounts: {
        id: {
          initialize: jasmine.createSpy('initialize'),
          renderButton: jasmine.createSpy('renderButton'),
          prompt: jasmine.createSpy('prompt'),
        },
      },
    };

    const spy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: Router, useValue: spy },
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('checkSession', () => {
    it('should set currentUser signal and isAuthenticated$ to true on success', () => {
      const mockUser: UserSessionDto = {
        id: '1',
        email: 'test@test.com',
        name: 'John Doe',
      } as UserSessionDto;
      let returnedUser: UserSessionDto | null | undefined;

      service.checkSession().subscribe((user) => {
        returnedUser = user;
      });

      const req = httpMock.expectOne(`${apiUrl}/me`);
      expect(req.request.method).toBe('GET');
      expect(req.request.withCredentials).toBeTrue();

      req.flush(mockUser);

      expect(returnedUser).toEqual(mockUser);
      expect(service.currentUser()).toEqual(mockUser);

      let isAuth: boolean | undefined;
      service.isAuthenticated$.subscribe((val) => (isAuth = val)).unsubscribe();
      expect(isAuth).toBeTrue();
    });

    it('should set currentUser signal to null and isAuthenticated$ to false on error', () => {
      let returnedUser: UserSessionDto | null | undefined = undefined;
      service.currentUser.set({ id: '1' } as UserSessionDto);

      service.checkSession().subscribe((user) => {
        returnedUser = user;
      });

      const req = httpMock.expectOne(`${apiUrl}/me`);
      req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

      expect(returnedUser).toBeNull();
      expect(service.currentUser()).toBeNull();

      let isAuth: boolean | undefined;
      service.isAuthenticated$.subscribe((val) => (isAuth = val)).unsubscribe();
      expect(isAuth).toBeFalse();
    });
  });

  describe('login', () => {
    it('should make a POST request and chain a checkSession call on success', () => {
      const mockCredentials: UserCredentials = {
        email: 'test@test.com',
        password: 'password123',
      } as UserCredentials;
      const mockResponse: AuthResponse = { token: 'mock-token-123' } as AuthResponse;
      const mockUser: UserSessionDto = { id: '1', email: 'test@test.com' } as UserSessionDto;

      service.login(mockCredentials).subscribe();

      const loginReq = httpMock.expectOne(`${apiUrl}/login`);
      expect(loginReq.request.method).toBe('POST');
      expect(loginReq.request.body).toEqual(mockCredentials);
      expect(loginReq.request.withCredentials).toBeTrue();

      loginReq.flush(mockResponse);

      const meReq = httpMock.expectOne(`${apiUrl}/me`);
      expect(meReq.request.method).toBe('GET');

      meReq.flush(mockUser);

      expect(service.currentUser()).toEqual(mockUser);
    });
  });

  describe('register', () => {
    it('should make a POST request and chain a checkSession call on success', () => {
      const mockRegisterData = {
        email: 'test@test.com',
        password: 'password123',
        name: 'John Doe',
      };
      const mockResponse: AuthResponse = { token: 'mock-token-123' } as AuthResponse;
      const mockUser: UserSessionDto = { id: '1', name: 'John Doe' } as UserSessionDto;

      service.register(mockRegisterData).subscribe();

      const registerReq = httpMock.expectOne(`${apiUrl}/register`);
      expect(registerReq.request.method).toBe('POST');
      expect(registerReq.request.body).toEqual(mockRegisterData);
      expect(registerReq.request.withCredentials).toBeTrue();

      registerReq.flush(mockResponse);

      const meReq = httpMock.expectOne(`${apiUrl}/me`);
      expect(meReq.request.method).toBe('GET');

      meReq.flush(mockUser);

      expect(service.currentUser()).toEqual(mockUser);
    });
  });

  describe('logout', () => {
    it('should clear states, make a POST request, and redirect to login on success', () => {
      service.currentUser.set({ id: '1' } as UserSessionDto);

      service.logout();

      const req = httpMock.expectOne(`${apiUrl}/logout`);
      expect(req.request.method).toBe('POST');
      expect(req.request.withCredentials).toBeTrue();

      req.flush({});

      expect(service.currentUser()).toBeNull();

      let isAuth: boolean | undefined;
      service.isAuthenticated$.subscribe((val) => (isAuth = val)).unsubscribe();
      expect(isAuth).toBeFalse();

      expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
    });

    it('should clear states and redirect to login even if the API request fails', () => {
      service.currentUser.set({ id: '1' } as UserSessionDto);

      service.logout();

      const req = httpMock.expectOne(`${apiUrl}/logout`);
      req.flush('Server Error', { status: 500, statusText: 'Internal Server Error' });

      expect(service.currentUser()).toBeNull();
      expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
    });
  });
});
