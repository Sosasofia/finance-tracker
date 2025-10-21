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
  private resizeHandler: () => void = () => {
    // update legend position responsively without recreating the chart
    try {
      if (!this.chartInstance) return;
      const canvas = this.canvasRef?.nativeElement;
      if (!canvas) return;
      const pos = this.getLegendPosition(
        canvas.clientWidth || window.innerWidth,
      );
      // only update if changed
      const opts = this.chartInstance.options as any;
      if (
        opts &&
        opts.plugins &&
        opts.plugins.legend &&
        opts.plugins.legend.position !== pos
      ) {
        opts.plugins.legend.position = pos;
        this.chartInstance.update();
      }
    } catch (e) {
      console.log("[CategoryChart] error handling resize", e);
    }
  };

  ngAfterViewInit(): void {
    // render after view init
    setTimeout(() => this.render(), 0);
    window.addEventListener("resize", this.resizeHandler);
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  ngOnChanges(changes: SimpleChanges): void {
    // Re-render when inputs change (labels/values)
    // Defer to allow view to be ready
    setTimeout(() => this.render(), 0);
  }

  ngOnDestroy(): void {
    try {
      this.chartInstance?.destroy();
    } catch (e) {
      console.log("[CategoryChart] error destroying chart", e);
    }
    try {
      window.removeEventListener("resize", this.resizeHandler);
    } catch (e) {
      console.log("[CategoryChart] error removing resize listener", e);
    }
  }

  private render(): void {
    const canvas = this.canvasRef?.nativeElement;
    if (!canvas) return;

    try {
      this.chartInstance?.destroy();
    } catch (e) {
      console.log("[CategoryChart] error destroying chart", e);
    }

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
      // compute totals for percentages
      const total = finalValues.reduce((s, v) => s + (Number(v) || 0), 0) || 0;

      // decide legend position based on available width
      const legendPos = this.getLegendPosition(
        canvas.clientWidth || window.innerWidth,
      );

      this.chartInstance = new Chart(ctx, {
        type: "pie",
        data,
        options: {
          plugins: {
            legend: {
              position: legendPos,
              align: "center",
              labels: {
                boxWidth: 12,
                padding: 8,
                font: { size: 14 },
                generateLabels: (chart: any) => {
                  const d = chart.data;
                  if (!d || !d.labels) return [];
                  return d.labels.map((label: any, i: number) => {
                    const value =
                      (d.datasets && d.datasets[0] && d.datasets[0].data[i]) ||
                      0;
                    const pct =
                      total > 0 ? Math.round((Number(value) / total) * 100) : 0;
                    const text = `${label} (${pct}%)`;
                    return {
                      text,
                      fillStyle:
                        (d.datasets &&
                          d.datasets[0] &&
                          d.datasets[0].backgroundColor &&
                          d.datasets[0].backgroundColor[i]) ||
                        "#000",
                      hidden: false,
                      index: i,
                    };
                  });
                },
              },
            },
            tooltip: {
              enabled: !disableTooltips,
              callbacks: {
                label: (context: any) => {
                  const v =
                    context.raw ??
                    (context.dataset &&
                      context.dataset.data &&
                      context.dataset.data[context.dataIndex]);
                  const pct =
                    total > 0 ? Math.round((Number(v) / total) * 100) : 0;
                  return `${context.label}: ${v} (${pct}%)`;
                },
              },
            },
          },
        },
      });
    } catch (err) {
      console.error("[CategoryChart] error creating chart", err);
    }
  }

  private getLegendPosition(width: number): "right" | "bottom" {
    return width >= 700 ? "right" : "bottom";
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
