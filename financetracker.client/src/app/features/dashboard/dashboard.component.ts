import { BreakpointObserver } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, computed, inject, signal, TemplateRef } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { map } from 'rxjs';

import { TransactionService } from '../../core/services/transaction.service';
import { CategoryFilterComponent } from '../../shared/components/category-filter/category-filter.component';
import { LoadingComponent } from '../../shared/components/loading/loading.component';
import { TransactionType } from '../../shared/models/transaction.model';
import { AddTransactionDialogComponent } from '../transactions/coomponents/add-transaction-dialog/add-transaction-dialog.component';
import { TransactionStore } from '../transactions/state/transaction.store';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  imports: [
    CommonModule,
    MatTableModule,
    LoadingComponent,
    FormsModule,
    MatIconModule,
    MatButtonToggleModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatInputModule,
    MatMenuModule,
    MatSnackBarModule,
    MatDialogModule,
    MatButtonModule,
    CategoryFilterComponent,
  ],
})
export class DashboardComponent {
  public TransactionTypeEnum = TransactionType;

  readonly store = inject(TransactionStore);
  readonly nameFilter = signal('');
  readonly activeType = signal<TransactionType | 'All'>('All');
  readonly activeCategory = signal<string>('All');

  readonly uniqueCategories = computed(() => {
    const txs = this.store.transactions();
    const cats = new Set<string>();
    for (const t of txs) {
      cats.add((t as any)?.category?.name ?? 'Uncategorized');
    }
    return ['All', ...Array.from(cats)];
  });

  readonly timeframe = signal<'this' | 'last' | 'all'>('this');

  private snackBar = inject(MatSnackBar);
  private transactionService = inject(TransactionService);
  private cdr = inject(ChangeDetectorRef);
  private breakpointObserver = inject(BreakpointObserver);

  isDownloading = false;

  isMobile = toSignal(
    this.breakpointObserver.observe('(max-width: 900px)').pipe(map((result) => result.matches)),
    { initialValue: false }
  );

  private dialog = inject(MatDialog);

  currentDialogRef: MatDialogRef<any> | null = null;

  openDialog(template: TemplateRef<any>) {
    this.currentDialogRef = this.dialog.open(template, {
      width: '90%',
      maxWidth: '400px',
      panelClass: 'rounded-dialog',
    });
  }

  closeDialog() {
    this.currentDialogRef?.close();
  }

  openAddTransaction(type: TransactionType): void {
    const dialogRef = this.dialog.open(AddTransactionDialogComponent, {
      width: '90%',
      maxWidth: '500px',
      panelClass: 'rounded-dialog',
      data: { transactionType: type },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result?.success) {
        console.log('Transaction added:', result.newTransaction);
      }
    });
  }

  private readonly today = new Date();
  readonly dateFrom = signal<Date | null>(null);
  readonly dateTo = signal<Date | null>(null);

  readonly baseTransactions = computed(() => {
    const transactions = this.store.transactions();
    const name = this.nameFilter().trim().toLowerCase();
    const type = this.activeType();
    const from = this.dateFrom();
    const to = this.dateTo();

    return transactions.filter((t) => {
      const matchesName =
        !name ||
        t.name.toLowerCase().includes(name) ||
        (t.description?.toLowerCase().includes(name) ?? false);
      const matchesType = type === 'All' || t.type.toLowerCase() === type.toLowerCase();

      const txDate = new Date(t.date);
      const matchesDateRange = (!from || txDate >= from) && (!to || txDate <= to);

      return matchesName && matchesType && matchesDateRange;
    });
  });

  readonly filteredTransactions = computed(() => {
    const category = this.activeCategory();
    return this.baseTransactions().filter((t) => {
      const catName = (t as any)?.category?.name ?? 'Uncategorized';
      return category === 'All' || catName === category;
    });
  });

  readonly dashboardMetrics = computed(() => {
    const txs = this.baseTransactions();
    let monthIncome = 0;
    let monthExpense = 0;
    const expenseTotals = new Map<string, number>();
    const incomeTotals = new Map<string, number>();

    for (const t of txs) {
      const categoryName = (t as any)?.category?.name ?? 'Uncategorized';

      if (t.type === TransactionType.Income) {
        monthIncome += t.amount;
        incomeTotals.set(categoryName, (incomeTotals.get(categoryName) ?? 0) + t.amount);
      } else if (t.type === TransactionType.Expense) {
        monthExpense += t.amount;
        expenseTotals.set(categoryName, (expenseTotals.get(categoryName) ?? 0) + t.amount);
      }
    }

    const totalsToShow = expenseTotals.size > 0 ? expenseTotals : incomeTotals;

    return {
      balance: monthIncome - monthExpense,
      monthIncome,
      monthExpense,
      chartData: {
        labels: Array.from(totalsToShow.keys()),
        values: Array.from(totalsToShow.values()),
      },
    };
  });

  readonly topCardDisplay = computed(() => {
    const category = this.activeCategory();
    const metrics = this.dashboardMetrics();

    if (category === 'All') {
      return {
        title: 'Total Balance',
        subtitle: 'Across all accounts',
        amount: metrics.balance,
      };
    } else {
      const isExpenseCategory = metrics.monthExpense >= metrics.monthIncome;

      return {
        title: category,
        subtitle: isExpenseCategory ? 'Total you spend' : 'Total you earned',
        amount: isExpenseCategory ? metrics.monthExpense : metrics.monthIncome,
      };
    }
  });

  filterByType(type: TransactionType | 'All'): void {
    this.activeType.set(type);
  }

  onCloseNameFilter(): void {
    this.nameFilter.set('');
  }

  clearDateFilters(): void {
    this.dateFrom.set(null);
    this.dateTo.set(null);
  }

  setDateRange(from: Date | null, to: Date | null): void {
    this.dateFrom.set(from);
    this.dateTo.set(to);
  }

  exportTransactions(format: 'csv' | 'xlsx' = 'csv'): void {
    if (this.isDownloading) return;

    this.isDownloading = true;
    this.cdr.detectChanges();

    const params: Record<string, any> = {};

    const fromVal = this.dateFrom();
    const toVal = this.dateTo();
    const filteredTxs = this.filteredTransactions();

    if (fromVal) params['dateFrom'] = this.formatDateToMMDDYYYY(fromVal);
    if (toVal) params['dateTo'] = this.formatDateToMMDDYYYY(toVal);

    if (!fromVal && !toVal && filteredTxs.length > 0) {
      const times = filteredTxs.map((t) => new Date(t.date).getTime());
      params['dateFrom'] = this.formatDateToMMDDYYYY(new Date(Math.min(...times)));
      params['dateTo'] = this.formatDateToMMDDYYYY(new Date(Math.max(...times)));
    }

    const exportRequest$ =
      format === 'xlsx'
        ? this.transactionService.exportExcel(params)
        : this.transactionService.exportCsv(params);

    exportRequest$.subscribe({
      next: (blob: Blob) => {
        this.downloadBlob(blob, `transactions_${this.fileDateSuffix()}.${format}`);
        this.isDownloading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        if (err.status === 429) {
          const seconds = err.retryAfterSeconds || 60;

          this.snackBar.open(err.message, 'Dismiss', { duration: 5000 });

          setTimeout(() => {
            this.isDownloading = false;
            this.cdr.detectChanges();
          }, seconds * 1000);
        } else {
          this.isDownloading = false;
          this.cdr.detectChanges();
          this.downloadCsvClient();
        }
      },
    });
  }

  private fileDateSuffix(): string {
    const now = new Date();
    return `${now.getFullYear()}${(now.getMonth() + 1).toString().padStart(2, '0')}${now
      .getDate()
      .toString()
      .padStart(2, '0')}`;
  }

  private formatDateToMMDDYYYY(date: Date | string | null | undefined): string {
    if (!date) return '';
    const d = new Date(date);
    const mm = (d.getMonth() + 1).toString().padStart(2, '0');
    const dd = d.getDate().toString().padStart(2, '0');
    const yyyy = d.getFullYear();
    return `${mm}/${dd}/${yyyy}`;
  }

  private downloadBlob(blob: Blob, fileName: string): void {
    if ((navigator as any).msSaveBlob) {
      (navigator as any).msSaveBlob(blob, fileName);
      return;
    }

    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
  }

  private downloadCsvClient(): void {
    const items = this.filteredTransactions();

    const headers = ['Name', 'Date', 'Description', 'Category', 'Type', 'Amount'];
    const rows = items.map((t) => [
      this.escapeCsv(t.name),
      this.formatDateToMMDDYYYY(t.date),
      this.escapeCsv(t.description ?? ''),
      this.escapeCsv((t as any)?.category?.name ?? t.categoryId ?? ''),
      t.type,
      t.amount.toFixed(2),
    ]);

    const csvArray = [headers, ...rows].map((r) => r.join(','));
    const csvContent = '\uFEFF' + csvArray.join('\r\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const fileName = `transactions_${this.fileDateSuffix()}.csv`;
    const link = document.createElement('a');
    if ((navigator as any).msSaveBlob) {
      (navigator as any).msSaveBlob(blob, fileName);
    } else {
      const url = window.URL.createObjectURL(blob);
      link.setAttribute('href', url);
      link.setAttribute('download', fileName);
      link.style.visibility = 'hidden';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    }
  }

  private escapeCsv(value: any): string {
    if (value == null) return '';
    const s = String(value);
    if (s.includes(',') || s.includes('"') || s.includes('\n') || s.includes('\r')) {
      return '"' + s.replace(/"/g, '""') + '"';
    }
    return s;
  }
}
