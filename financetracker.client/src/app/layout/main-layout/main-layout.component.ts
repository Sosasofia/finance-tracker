import { Component } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatIconModule } from "@angular/material/icon";
import { MatListModule } from "@angular/material/list";
import { MatSidenav, MatSidenavModule } from "@angular/material/sidenav";
import { MatToolbarModule } from "@angular/material/toolbar";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";

import { AuthService } from "../../core/auth/auth.service";

@Component({
  selector: "app-main-layout",
  standalone: true,
  templateUrl: "./main-layout.component.html",
  styleUrl: "./main-layout.component.css",
  imports: [
    CommonModule,
    RouterModule,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatSidenav,
  ],
  schemas: [],
})

export class MainLayoutComponent {
  isExpanded = true;
  constructor(private authService: AuthService) {}

  logout(): void {
    this.authService.logout();
  }

  toggleSidenav() {
    this.isExpanded = !this.isExpanded;
  }
}
