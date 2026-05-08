import { Component, input } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-loading',
  template: `
    @if (loading()) {
      <div class="overlay loading-spinner">
        <mat-spinner [diameter]="diameter()"></mat-spinner>
      </div>
    }
  `,
  styleUrls: ['./loading.component.css'],
  imports: [MatProgressSpinnerModule],
})
export class LoadingComponent {
  loading = input(false);
  diameter = input(50);
}
