import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { UserCredentials } from '../../models/user-credentials.model';
import { LoginComponent } from './login.component';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockAuthService = jasmine.createSpyObj('AuthService', ['login']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      declarations: [],
      imports: [LoginComponent, HttpClientTestingModule],
      providers: [
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display a title', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h2')?.textContent).toContain('Login');
  });

  it('should initialize the form with empty values', () => {
    expect(component.form.value).toEqual({
      username: '',
      password: '',
    });
  });

  it('should call AuthService.Login on form submit', () => {
    mockAuthService.login.and.returnValue(of({ token: 'fake-token', username: 'testuser' }));
    const mockCredentials: UserCredentials = {
      username: 'testuser',
      password: 'testpassword',
    };

    component.form.setValue(mockCredentials);
    component.onSubmit();

    expect(mockAuthService.login).toHaveBeenCalledWith(mockCredentials);
  });

  it('should navigate to /dashboard on successful login', () => {
    mockAuthService.login.and.returnValue(of({ token: 'fake-token', username: 'testuser' }));

    component.form.setValue({ username: 'test', password: '123' });
    component.onSubmit();

    expect(mockAuthService.login).toHaveBeenCalledWith({
      username: 'test',
      password: '123',
    });

    expect(mockRouter.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('should set errorMessage on login failure', () => {
    const mockFormValue = { username: 'test', password: '1234' };
    component.form.setValue(mockFormValue);

    const mockError = { error: 'Invalid credentials' };
    mockAuthService.login.and.returnValue(throwError(() => mockError));

    component.onSubmit();

    expect(mockAuthService.login).toHaveBeenCalledWith(mockFormValue);
    expect(component.errorMessage).toBe('Invalid credentials');
  });

  it('should log error if form is invalid', () => {
    const mockFormValue = { username: '', password: '' };
    component.form.setValue(mockFormValue);

    spyOn(console, 'log');
    component.onSubmit();

    expect(mockAuthService.login).not.toHaveBeenCalled();
    expect(console.log).toHaveBeenCalledWith('Form is invalid');
  });
});
