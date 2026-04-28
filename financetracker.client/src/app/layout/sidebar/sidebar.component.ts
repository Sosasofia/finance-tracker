import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterModule, MatIconModule, MatButtonModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css',
})
export class SidebarComponent {
  @Input({ required: true }) isExpanded!: boolean;
  @Input({ required: true }) navItems: any[] = [];
  @Input({ required: true }) userName!: string;

  @Output() logoutClicked = new EventEmitter<void>();

  onLogout(): void {
    this.logoutClicked.emit();
  }
}
