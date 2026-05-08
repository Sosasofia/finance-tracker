import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';

import { AuthService } from '../../core/auth/auth.service';
import { UserCredentials } from '../../shared/models/user-credentials.model';
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
      imports: [LoginComponent, NoopAnimationsModule],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display a title', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h2')?.textContent).toContain('Login');
  });

  it('should initialize the form with empty values', () => {
    expect(component.form.value).toEqual({
      email: '',
      password: '',
    });
  });

  it('should call AuthService.Login on form submit', () => {
    mockAuthService.login.and.returnValue(of({ token: 'fake-token', email: 'test@email.com' }));
    const mockCredentials: UserCredentials = {
      email: 'test@email.com',
      password: 'testpassword',
    };

    component.form.setValue(mockCredentials);
    component.onSubmit();

    expect(mockAuthService.login).toHaveBeenCalledWith(mockCredentials);
  });

  it('should navigate to /dashboard on successful login', () => {
    mockAuthService.login.and.returnValue(of({ token: 'fake-token', email: 'test@email.com' }));

    component.form.setValue({ email: 'test@email.com', password: 'testpassword' });
    component.onSubmit();

    expect(mockRouter.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('should set errorMessage on login failure', () => {
    const mockFormValue = { email: 'test@email.com', password: 'testpassword' };
    component.form.setValue(mockFormValue);

    const mockError = { error: 'Login failed. Please try again.' };
    mockAuthService.login.and.returnValue(throwError(() => mockError));

    component.onSubmit();

    expect(component.errorMessage).toBe('Login failed. Please try again.');
  });

  it('should not call authService.login if form is invalid', () => {
    component.form.controls['email'].setValue('not-an-email');
    component.onSubmit();

    expect(mockAuthService.login).not.toHaveBeenCalled();
  });
});
