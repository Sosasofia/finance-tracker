import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryChartComponent } from '../../shared/components/category-chart/category-chart.component';
import { TransactionStore } from '../transactions/state/transaction.store';

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [CommonModule, FormsModule, CategoryChartComponent],
  templateUrl: './analytics.component.html',
  styleUrl: './analytics.component.css',
})
export class AnalyticsComponent {
  readonly store = inject(TransactionStore);

  timeframe = signal<'this' | 'last' | 'all'>('this');
  transactionType = signal<'Income' | 'Expense'>('Expense');

  filteredData = computed(() => {
    const txs = this.store.transactions().filter((t) => t.type === this.transactionType());
    const now = new Date();
    const time = this.timeframe();

    return txs.filter((t) => {
      const txDate = new Date(t.date);
      if (time === 'this') {
        return txDate.getMonth() === now.getMonth() && txDate.getFullYear() === now.getFullYear();
      } else if (time === 'last') {
        const lastMonth = new Date(now.getFullYear(), now.getMonth() - 1, 1);
        return (
          txDate.getMonth() === lastMonth.getMonth() &&
          txDate.getFullYear() === lastMonth.getFullYear()
        );
      }
      return true;
    });
  });

  chartData = computed(() => {
    const data = this.filteredData();
    const totals = new Map<string, number>();
    let totalAmount = 0;

    for (const t of data) {
      const catName = (t as any)?.category?.name ?? t.categoryId ?? 'Uncategorized';
      totals.set(catName, (totals.get(catName) ?? 0) + t.amount);
      totalAmount += t.amount;
    }

    const categories = Array.from(totals.keys());
    const values = Array.from(totals.values());

    const details = categories
      .map((cat, index) => ({
        name: cat,
        amount: values[index],
      }))
      .sort((a, b) => b.amount - a.amount);

    return {
      labels: categories,
      values: values,
      details: details,
      totalAmount,
    };
  });

  setType(type: 'Income' | 'Expense') {
    this.transactionType.set(type);
  }
}
