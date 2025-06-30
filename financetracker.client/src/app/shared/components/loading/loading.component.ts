import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

@Component({
  selector: "app-loading",
  template: `
    <div *ngIf="loading" class="overlay loading-spinner">
      <mat-spinner [diameter]="diameter"></mat-spinner>
    </div>
  `,
  styleUrls: ["./loading.component.css"],
  imports: [CommonModule, MatProgressSpinnerModule],
})
export class LoadingComponent {
  @Input() loading = false;
  @Input() diameter = 50; 
}
