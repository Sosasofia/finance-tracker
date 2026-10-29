import { BreakpointObserver } from '@angular/cdk/layout';
import { DatePipe, DOCUMENT } from '@angular/common';
import { Component, effect, inject, signal, OnInit } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterModule } from '@angular/router';
import { map } from 'rxjs/operators';

import { AuthService } from '../../core/auth/auth.service';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { UserSettingsStateService } from '../../core/services/user-settings-state.service';
@Component({
  selector: 'app-main-layout',
  standalone: true,
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css',
  imports: [
    RouterModule,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    DatePipe,
    SidebarComponent,
  ],
})
export class MainLayoutComponent implements OnInit {
  private document = inject(DOCUMENT);
  private authService = inject(AuthService);
  private userSettingsState = inject(UserSettingsStateService);
  private breakpointObserver = inject(BreakpointObserver);

  userProfile = this.userSettingsState.profile;
  userPreferences = this.userSettingsState.preferences;

  currentDate = new Date();
  userName = signal('Developer');
  userAvatar = signal('...');
  userProvider = signal('local');
  userMemberSince = signal(new Date());
  mobileAvatarFailed = signal(false);

  constructor() {
    effect(() => {
      const profile = this.userProfile();
      if (!profile) return;

      this.userName.set(profile.name);
      this.userAvatar.set(profile.picture || '...');
      this.mobileAvatarFailed.set(false);
      this.userProvider.set(profile.provider);
      this.userMemberSince.set(profile.createdAt);
    });
  }

  ngOnInit() {
    this.userSettingsState.load().subscribe();
  }

  onMobileAvatarError(): void {
    this.mobileAvatarFailed.set(true);
  }

  isMobile = toSignal(
    this.breakpointObserver.observe('(max-width: 900px)').pipe(map((result) => result.matches)),
    { initialValue: false }
  );

  navItems = signal([
    { path: '/dashboard', icon: 'home', label: 'Dashboard', mobileOnly: false },
    { path: '/income', icon: 'keyboard_arrow_down', label: 'Income', mobileOnly: false },
    { path: '/expense', icon: 'keyboard_arrow_up', label: 'Expense', mobileOnly: false },
    { path: '/analytics', icon: 'pie_chart_outline', label: 'Analytics', mobileOnly: false },
    { path: '/settings', icon: 'settings', label: 'Settings', mobileOnly: false },
  ]);

  logout() {
    this.authService.logout().subscribe(() => {
      this.document.location.href = '/login';
    });
  }
}
