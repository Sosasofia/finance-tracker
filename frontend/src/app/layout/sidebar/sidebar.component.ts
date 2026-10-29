import { Component, EventEmitter, Input, Output, ChangeDetectionStrategy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterModule, MatIconModule, MatButtonModule],
  templateUrl: './sidebar.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './sidebar.component.css',
})
export class SidebarComponent {
  @Input({ required: true }) navItems: any[] = [];
  @Input({ required: true }) userName!: string;
  @Input() userEmail?: string;
  @Input() userPicture?: string;

  @Output() logoutClicked = new EventEmitter<void>();

  onLogout(): void {
    this.logoutClicked.emit();
  }
}
