import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
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
    CategoryChartComponent,
  ],
})
export class DashboardComponent implements OnInit {
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

  constructor(private transactionService: TransactionService) {}

  ngOnInit(): void {
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

    this.filteredTransactions = this.transactions.filter((t) => {
      const matchesName =
        !name ||
        t.name.toLowerCase().includes(name) ||
        (t.description?.toLowerCase().includes(name) ?? false);

      const matchesType =
        this.activeType === 'All' || t.type.toLowerCase() === this.activeType.toLowerCase();

      return matchesName && matchesType;
    });

    this.computeDashboardMetrics();
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

  // helper for testing: clear all filters and show all transactions
  showAllNoFilters(): void {
    this.timeframe = 'all';
    this.categoryFilter = '';
    this.nameFilter = '';
    this.activeType = 'All';
    this.applyFilters();
    this.computeDashboardMetrics();
  }
}
