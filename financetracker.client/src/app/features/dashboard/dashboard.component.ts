import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { MatTableModule } from "@angular/material/table";
import { MatIconModule } from "@angular/material/icon";
import { FormsModule } from "@angular/forms";
import { MatButtonToggleModule } from "@angular/material/button-toggle";

import { TransactionService } from "../../core/services/transaction.service";
import { Transaction } from "../../models/transaction.model";
import { LoadingComponent } from "../../shared/components/loading/loading.component";

@Component({
  selector: "app-dashboard",
  standalone: true,
  templateUrl: "./dashboard.component.html",
  styleUrl: "./dashboard.component.css",
  imports: [
    CommonModule,
    MatTableModule,
    LoadingComponent,
    FormsModule,
    MatIconModule,
    MatButtonToggleModule,
  ],
})
export class DashboardComponent implements OnInit {
  transactions: Transaction[] = [];
  filteredTransactions: Transaction[] = [];
  displayedColumns: string[] = [
    "name",
    "date",
    "description",
    "category",
    "amount",
  ];

  loading = false;
  nameFilter: string = "";
  activeType: "All" | "Income" | "Expense" = "All";

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
      },
      error: (err) => {
        console.error("Error fetching transactions", err);
        this.loading = false;
      },
    });
  }

  filterByType(type?: "Income" | "Expense"): void {
    this.activeType = type ?? "All";
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
        this.activeType === "All" ||
        t.type.toLowerCase() === this.activeType.toLowerCase();

      return matchesName && matchesType;
    });
  }

  onCloseNameFilter(): void {
    this.nameFilter = "";
    this.applyFilters();
  }
}
