import { BreakpointObserver } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { map } from 'rxjs';

import { MatDialog } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CategoryChartComponent } from '../../shared/components/category-chart/category-chart.component';
import { TransactionType } from '../../shared/models/transaction.model';
import { AddTransactionDialogComponent } from '../transactions/coomponents/add-transaction-dialog/add-transaction-dialog.component';
import { TransactionStore } from '../transactions/state/transaction.store';

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [CommonModule, FormsModule, CategoryChartComponent, MatIcon],
  templateUrl: './analytics.component.html',
  styleUrl: './analytics.component.css',
})
export class AnalyticsComponent {
  public TransactionTypeEnum = TransactionType;

  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  readonly store = inject(TransactionStore);

  timeframe = signal<'this' | 'last' | 'all'>('this');
  transactionType = signal<TransactionType>(TransactionType.Expense);

  private breakpointObserver = inject(BreakpointObserver);

  isDownloading = false;

  isMobile = toSignal(
    this.breakpointObserver.observe('(max-width: 900px)').pipe(map((result) => result.matches)),
    { initialValue: false }
  );

  filteredData = computed(() => {
    const txs = this.store
      .transactions()
      .filter((t) => t.type.toLowerCase() === this.transactionType().toLowerCase());

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

  setType(type: TransactionType) {
    this.transactionType.set(type);
  }

  openAddTransaction() {
    const dialogRef = this.dialog.open(AddTransactionDialogComponent, {
      width: '90%',
      maxWidth: '500px',
      panelClass: 'rounded-dialog',
      data: { transactionType: this.transactionType().toLowerCase() },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result?.success) {
        this.store.addTransactionLocal(result.newTransaction);
        this.snackBar.open('Transaction created successfully!', 'Close', { duration: 3000 });
      }
    });
  }
}
