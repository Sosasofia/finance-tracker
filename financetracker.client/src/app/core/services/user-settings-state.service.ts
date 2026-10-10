import { Injectable, inject, Injector, signal } from '@angular/core';
import { forkJoin, map, Observable, tap } from 'rxjs';
import { UserSettingsService } from './user-settings.service';
import { UserPreferences } from '../../shared/models/user-preferences.model';
import { UserProfile } from '../../shared/models/user-profile.model';

@Injectable({ providedIn: 'root' })
export class UserSettingsStateService {
  private readonly injector = inject(Injector);

  readonly profile = signal<UserProfile | null>(null);
  readonly preferences = signal<UserPreferences | null>(null);

  load(): Observable<[UserProfile, UserPreferences]> {
    const userSettingsService = this.injector.get(UserSettingsService);
    return forkJoin({
      profile: userSettingsService.getProfile(),
      preferences: userSettingsService.getPreferences(),
    }).pipe(
      tap(({ profile, preferences }) => {
        this.profile.set(profile);
        this.preferences.set(preferences);
      }),
      map(({ profile, preferences }) => [profile, preferences] as [UserProfile, UserPreferences])
    );
  }

  updateProfile(data: { name: string; profilePicture?: string }): Observable<UserProfile> {
    return this.injector
      .get(UserSettingsService)
      .updateProfile(data)
      .pipe(tap((profile) => this.profile.set(profile)));
  }

  updatePreferences(data: UserPreferences): Observable<UserPreferences> {
    return this.injector
      .get(UserSettingsService)
      .updatePreferences(data)
      .pipe(tap((preferences) => this.preferences.set(preferences)));
  }
}
