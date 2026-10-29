import { Component, input, output, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-category-filter',
  standalone: true,
  templateUrl: './category-filter.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./category-filter.component.css'],
})
export class CategoryFilterComponent {
  categories = input.required<string[]>();
  activeCategory = input<string>('All');

  categoryChange = output<string>();

  selectCategory(category: string) {
    this.categoryChange.emit(category);
  }

  onScroll(event: WheelEvent) {
    const container = event.currentTarget as HTMLElement;

    container.scrollLeft += event.deltaY;

    event.preventDefault();
  }
}
