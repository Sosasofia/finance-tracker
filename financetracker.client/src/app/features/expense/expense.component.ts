import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { MatTableModule } from "@angular/material/table";

import { TransactionService } from "../../core/services/transaction.service";
import { Transaction } from "../../models/transaction.model";
import { TransactionFormComponent } from "../transactions/transaction-form/transaction-form.component";
import { LoadingComponent } from "../../shared/components/loading/loading.component";

@Component({
  selector: "app-expense",
  standalone: true,
  templateUrl: "./expense.component.html",
  styleUrl: "./expense.component.css",
  imports: [
    TransactionFormComponent,
    CommonModule,
    MatTableModule,
    LoadingComponent,
  ],
})
export class ExpenseComponent implements OnInit {
  expenseTransactions: Transaction[] = [];
  displayedColumns: string[] = ["name", "date", "amount"];
  loading = false;

  constructor(private transactionService: TransactionService) {}

  ngOnInit(): void {
    this.loadExpenseTransactions();
  }

  loadExpenseTransactions(): void {
    this.loading = true;
    this.transactionService.getTransactions().subscribe({
      next: (transactions) => {
        this.expenseTransactions = transactions.filter(
          (t) => t.type.toLocaleLowerCase() === "expense",
        );
        this.loading = false;
      },
      error: (err) => {
        console.error("Error fetching expense transactions", err);
        this.loading = false;
      },
    });
  }

  handleFormSubmit(formData: Transaction): void {
    this.transactionService.createTransaction(formData).subscribe({
      next: (response) => {
        console.log("Transaction created successfully::", response);
        this.expenseTransactions.push(formData);
      },
      error: (error) => {
        console.error("Error creating the transaction:", error);
      },
      complete: () => {
        console.log("Transaction creation request completed");
      },
    });
  }
}
