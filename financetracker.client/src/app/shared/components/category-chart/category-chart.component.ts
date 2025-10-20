import { CommonModule } from "@angular/common";
import {
  Component,
  Input,
  AfterViewInit,
  OnDestroy,
  ElementRef,
  ViewChild,
  OnChanges,
  SimpleChanges,
} from "@angular/core";
import Chart from "chart.js/auto";

@Component({
  selector: "app-category-chart",
  standalone: true,
  imports: [CommonModule],
  templateUrl: "./category-chart.component.html",
  styleUrls: ["./category-chart.component.css"],
})
export class CategoryChartComponent
  implements AfterViewInit, OnDestroy, OnChanges
{
  @Input() labels: string[] = [];
  @Input() values: number[] = [];
  @Input() ariaLabel = "Category pie chart";

  @ViewChild("canvas") canvasRef?: ElementRef<HTMLCanvasElement>;
  private chartInstance: any;

  ngAfterViewInit(): void {
    // render after view init
    setTimeout(() => this.render(), 0);
  }

  ngOnChanges(changes: SimpleChanges): void {
    // Re-render when inputs change (labels/values)
    // Defer to allow view to be ready
    setTimeout(() => this.render(), 0);
  }

  ngOnDestroy(): void {
    try {
      this.chartInstance?.destroy();
    } catch (e) {}
  }

  private render(): void {
    const canvas = this.canvasRef?.nativeElement;
    if (!canvas) return;

    try {
      this.chartInstance?.destroy();
    } catch (e) {}

    const labels = this.labels ?? [];
    const values = this.values ?? [];

    // Copy inputs and coerce values to numbers
    let finalLabels = labels.slice();
    let finalValues = values.slice().map((v: any) => Number(v) || 0);
    let backgroundColors = finalLabels.map((_, i) => this.pickColor(i));
    let disableTooltips = false;

    // If there are no values or all values are zero, show the "No data" fallback
    const allZero = finalValues.length > 0 && finalValues.every((v) => v === 0);
    if (!finalValues.length || allZero) {
      finalLabels = ["No data"];
      finalValues = [1];
      backgroundColors = ["#e0e0e0"];
      disableTooltips = true;
    }

    const data = {
      labels: finalLabels,
      datasets: [{ data: finalValues, backgroundColor: backgroundColors }],
    };

    try {
      const ctx = canvas.getContext("2d");
      if (!ctx) return;
      this.chartInstance = new Chart(ctx, {
        type: "pie",
        data,
        options: {
          plugins: {
            legend: { position: "bottom" },
            tooltip: { enabled: !disableTooltips },
          },
        },
      });
    } catch (err) {
      console.error("[CategoryChart] error creating chart", err);
    }
  }

  private pickColor(index: number): string {
    const palette = [
      "#4caf50",
      "#f44336",
      "#2196f3",
      "#ff9800",
      "#9c27b0",
      "#03a9f4",
      "#8bc34a",
      "#ffc107",
      "#e91e63",
      "#00bcd4",
    ];
    return palette[index % palette.length];
  }
}
