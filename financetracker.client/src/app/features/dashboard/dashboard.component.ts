import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { MatTableModule } from "@angular/material/table";

import { TransactionService } from "../../core/services/transaction.service";
import { Transaction } from "../../models/transaction.model";
import { LoadingComponent } from "../../shared/components/loading/loading.component";

@Component({
  selector: "app-dashboard",
  standalone: true,
  templateUrl: "./dashboard.component.html",
  styleUrl: "./dashboard.component.css",
  imports: [CommonModule, MatTableModule, LoadingComponent],
})
export class DashboardComponent implements OnInit {
  transactions: Transaction[] = [];
  displayedColumns: string[] = [
    "name",
    "date",
    "description",
    "category",
    "amount",
  ];
  loading = false;

  constructor(private transactionService: TransactionService) {}

  ngOnInit(): void {
    this.loadTransactions();
  }

  loadTransactions(): void {
    this.loading = true;
    this.transactionService.getTransactions().subscribe({
      next: (data: Transaction[]) => {
        this.transactions = data;
        this.loading = false;
      },
      error: (err) => {
        console.error("Error fetching transactions", err);
        this.loading = false;
      },
    });
  }
}
