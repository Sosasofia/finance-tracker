import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  styleUrl: './app.component.css',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [RouterModule],
})
export class AppComponent {
  title = 'frontend';
}
