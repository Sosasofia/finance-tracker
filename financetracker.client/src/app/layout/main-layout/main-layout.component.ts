import { BreakpointObserver } from '@angular/cdk/layout';
import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
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
  ],
})
export class MainLayoutComponent {
  private authService = inject(AuthService);
  private breakpointObserver = inject(BreakpointObserver);
  currentDate = new Date();

  userName = signal('Developer');

  isMobile = toSignal(
    this.breakpointObserver.observe('(max-width: 900px)').pipe(map((result) => result.matches)),
    { initialValue: false }
  );

  isExpanded = signal(true);

  navItems = signal([
    { path: '/dashboard', icon: 'home', label: 'Dashboard', mobileOnly: false },
    { path: '/income', icon: 'keyboard_arrow_down', label: 'Income', mobileOnly: false },
    { path: '/expense', icon: 'keyboard_arrow_up', label: 'Expense', mobileOnly: false },
    { path: '/analytics', icon: 'pie_chart', label: 'Analytics', mobileOnly: true },
  ]);

  toggleSidenav() {
    this.isExpanded.update((v) => !v);
  }

  logout(): void {
    this.authService.logout();
  }
}
