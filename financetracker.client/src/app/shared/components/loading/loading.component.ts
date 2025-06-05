import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

@Component({
  selector: "app-loading",
  template: `
    <div *ngIf="loading" class="overlay">
      <mat-spinner [diameter]="50"></mat-spinner>
    </div>
  `,
  styleUrl: "./loading.component.css",
  imports: [CommonModule, MatProgressSpinnerModule],
})
export class LoadingComponent {
  @Input() loading = false;
}
