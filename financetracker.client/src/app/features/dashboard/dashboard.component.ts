import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { FormsModule } from '@angular/forms';
import { MatButtonToggleModule } from '@angular/material/button-toggle';

import { TransactionService } from '../../core/services/transaction.service';
import { Transaction } from '../../models/transaction.model';
import { LoadingComponent } from '../../shared/components/loading/loading.component';
import { CategoryChartComponent } from '../../shared/components/category-chart/category-chart.component';

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
    CategoryChartComponent,
  ],
})
export class DashboardComponent implements OnInit {
  private transactionService: TransactionService = inject(TransactionService);

  transactions: Transaction[] = [];
  filteredTransactions: Transaction[] = [];
  displayedColumns: string[] = ['name', 'date', 'description', 'category', 'amount'];

  loading = false;
  nameFilter = '';
  activeType: 'All' | 'Income' | 'Expense' = 'All';

  // Dashboard metrics
  balance = 0;
  monthIncome = 0;
  monthExpense = 0;

  // Chart filters
  timeframe: 'this' | 'last' | 'all' = 'this';
  categoryFilter = '';
  recentTransactions: Transaction[] = [];
  categoryChartData: { labels: string[]; values: number[] } = {
    labels: [],
    values: [],
  };

  // Date range filters
  dateFrom: Date | null = null;
  dateTo: Date | null = null;
  exportFormat: 'csv' | 'xlsx' = 'csv';

  ngOnInit(): void {
    this.setDateRange(this.dateFrom, this.dateTo);
    this.loadTransactions();
  }

  loadTransactions(): void {
    this.loading = true;
    this.transactionService.getTransactions().subscribe({
      next: (data: Transaction[]) => {
        this.transactions = data;
        this.applyFilters();
        this.loading = false;
        this.recentTransactions = this.transactions.slice(0, 9);
        this.computeDashboardMetrics();
      },
      error: (err) => {
        console.error('Error fetching transactions', err);
        this.loading = false;
      },
    });
  }

  filterByType(type?: 'Income' | 'Expense'): void {
    this.activeType = type ?? 'All';
    this.applyFilters();
  }

  applyFilters(): void {
    const name = this.nameFilter.trim().toLowerCase();

    let from = this.dateFrom ? new Date(this.dateFrom) : null;
    let to = this.dateTo ? new Date(this.dateTo) : null;

    if (from) {
      from = new Date(from.getFullYear(), from.getMonth(), from.getDate(), 0, 0, 0, 0);
    }
    if (to) {
      to = new Date(to.getFullYear(), to.getMonth(), to.getDate(), 23, 59, 59, 999);
    }

    this.filteredTransactions = this.transactions.filter((t) => {
      const matchesName =
        !name ||
        t.name.toLowerCase().includes(name) ||
        (t.description?.toLowerCase().includes(name) ?? false);

      const matchesType =
        this.activeType === 'All' || t.type.toLowerCase() === this.activeType.toLowerCase();

      // const matchesDateRange =
      //   (!this.dateFrom || new Date(t.date) >= this.dateFrom) &&
      //   (!this.dateTo || new Date(t.date) <= this.dateTo);
      const txDate = new Date(t.date);
      const matchesDateRange = (!from || txDate >= from) && (!to || txDate <= to);

      return matchesName && matchesType && matchesDateRange;
    });
  }

  computeDashboardMetrics(): void {
    const now = new Date();
    const lastMonth = new Date(now.getFullYear(), now.getMonth() - 1, 1);

    const expenseTotals = new Map<string, number>();
    const incomeTotals = new Map<string, number>();

    this.monthIncome = 0;
    this.monthExpense = 0;

    for (const t of this.transactions) {
      const transactionDate = new Date(t.date);

      if (!this.matchesTimeframe(transactionDate, now, lastMonth)) {
        continue;
      }

      const categoryName = (t as any)?.category?.name ?? 'Uncategorized';

      if (t.type === 'Income') {
        this.monthIncome += t.amount;
        const currentTotal = incomeTotals.get(categoryName) ?? 0;
        incomeTotals.set(categoryName, currentTotal + t.amount);
      } else if (t.type === 'Expense') {
        this.monthExpense += t.amount;
        const currentTotal = expenseTotals.get(categoryName) ?? 0;
        expenseTotals.set(categoryName, currentTotal + t.amount);
      }
    }

    this.balance = this.monthIncome - this.monthExpense;
    this.generateChartData(expenseTotals, incomeTotals);
  }

  private matchesTimeframe(date: Date, now: Date, lastMonth: Date): boolean {
    if (this.timeframe === 'all') return true;

    const year = date.getFullYear();
    const month = date.getMonth();

    if (this.timeframe === 'this') {
      return year === now.getFullYear() && month === now.getMonth();
    }
    // 'last'
    return year === lastMonth.getFullYear() && month === lastMonth.getMonth();
  }

  private generateChartData(
    expenseTotals: Map<string, number>,
    incomeTotals: Map<string, number>
  ): void {
    const totalsToShow = expenseTotals.size > 0 ? expenseTotals : incomeTotals;

    this.categoryChartData = {
      labels: Array.from(totalsToShow.keys()),
      values: Array.from(totalsToShow.values()),
    };
  }

  onCloseNameFilter(): void {
    this.nameFilter = '';
    this.applyFilters();
  }

  clearDateFilters(): void {
    this.dateFrom = null;
    this.dateTo = null;
    this.applyFilters();
  }

  setDateRange(from: Date | null, to: Date | null): void {
    if (from && to) {
      this.dateFrom = from;
      this.dateTo = to;
      return;
    }
    // Default to current month
    const today = new Date();
    this.dateFrom = new Date(today.getFullYear(), today.getMonth(), 1);
    this.dateTo = new Date(today.getFullYear(), today.getMonth() + 1, 0);
  }

  exportTransactions(format: 'csv' | 'xlsx' = 'csv'): void {
    const params: Record<string, any> = {};

    if (this.dateFrom) params['dateFrom'] = this.formatDateToMMDDYYYY(this.dateFrom);
    if (this.dateTo) params['dateTo'] = this.formatDateToMMDDYYYY(this.dateTo);

    if (!this.dateFrom && !this.dateTo && this.filteredTransactions.length > 0) {
      const times = this.filteredTransactions.map((t) => new Date(t.date).getTime());
      const minDate = new Date(Math.min(...times));
      const maxDate = new Date(Math.max(...times));
      params['dateFrom'] = this.formatDateToMMDDYYYY(minDate);
      params['dateTo'] = this.formatDateToMMDDYYYY(maxDate);
    }

    // If backend endpoints are available, request the server to produce the CSV/XLSX
    if (format === 'xlsx') {
      this.transactionService.exportExcel(params).subscribe({
        next: (blob: Blob) => {
          const fileName = `transactions_${this.fileDateSuffix()}.xlsx`;
          this.downloadBlob(blob, fileName);
        },
        error: (err) => {
          console.error('Server-side xlsx export failed, falling back to client CSV', err);
          // fallback to client CSV
          this.downloadCsvClient();
        },
      });
    } else {
      // CSV
      this.transactionService.exportCsv(params).subscribe({
        next: (blob: Blob) => {
          const fileName = `transactions_${this.fileDateSuffix()}.csv`;
          this.downloadBlob(blob, fileName);
        },
        error: (err) => {
          console.error('Server-side CSV export failed, falling back to client CSV', err);
          this.downloadCsvClient();
        },
      });
    }
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
    const items = this.filteredTransactions;

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
    // prepend UTF-8 BOM so Excel detects UTF-8
    const csvContent = '\uFEFF' + csvArray.join('\r\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const fileName = `transactions_${this.fileDateSuffix()}.csv`;
    const link = document.createElement('a');
    if ((navigator as any).msSaveBlob) {
      // IE 10+
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
    // escape quotes and wrap in quotes if necessary
    if (s.includes(',') || s.includes('"') || s.includes('\n') || s.includes('\r')) {
      return '"' + s.replace(/"/g, '""') + '"';
    }
    return s;
  }
}
