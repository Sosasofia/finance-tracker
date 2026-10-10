import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserProfile } from '../../shared/models/user-profile.model';
import { UserPreferences } from '../../shared/models/user-preferences.model';

interface UpdateProfileRequest {
  name: string;
  profilePicture?: string;
}

interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

@Injectable({
  providedIn: 'root',
})
export class UserSettingsService {
  private readonly apiUrl = environment.apiUrl + '/users/me';

  constructor(private http: HttpClient) {}

  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/profile`);
  }

  updateProfile(data: UpdateProfileRequest): Observable<UserProfile> {
    return this.http.put<UserProfile>(`${this.apiUrl}/profile`, data);
  }

  changePassword(data: ChangePasswordRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/password`, data);
  }

  deleteAccount(): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}`);
  }

  getPreferences(): Observable<UserPreferences> {
    return this.http.get<UserPreferences>(`${this.apiUrl}/preferences`);
  }

  updatePreferences(data: UserPreferences): Observable<UserPreferences> {
    return this.http.put<UserPreferences>(`${this.apiUrl}/preferences`, data);
  }
}
