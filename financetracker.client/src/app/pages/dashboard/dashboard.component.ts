import { CommonModule, DatePipe } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { Transaction } from "../../models/transaction.model";
import { TransactionService } from "../../services/transaction.service";

@Component({
  selector: "app-dashboard",
  standalone: true,
  templateUrl: "./dashboard.component.html",
  styleUrl: "./dashboard.component.css",
  imports: [CommonModule],
})
export class DashboardComponent implements OnInit {
  transactions: Transaction[] = [];

  constructor(
    private transactionService: TransactionService,
    private datePipe: DatePipe,
  ) {}

  ngOnInit(): void {
    this.loadTransactions();
  }

  loadTransactions(): void {
    this.transactionService.getTransactions().subscribe({
      next: (data: Transaction[]) => {
        this.transactions = data;
      },
      error: (err) => console.error("Error fetching transactions", err),
    });
  }
}
