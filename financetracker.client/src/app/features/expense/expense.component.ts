import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
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
export class ExpenseComponent implements OnInit, OnDestroy {
  @ViewChild(TransactionFormComponent) transactionFormChild!: TransactionFormComponent;

  expenseTransactions: Transaction[] = [];
  displayedColumns: string[] = ["name", "date", "amount"];
  loading = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  private successTimeout: any; 

  constructor(private transactionService: TransactionService) {}

  ngOnInit(): void {
    this.loadExpenseTransactions();
  }

   ngOnDestroy(): void {
    if (this.successTimeout) {
      clearTimeout(this.successTimeout);
    }
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
    this.errorMessage = null;
    this.successMessage = null;

    this.transactionService.createTransaction(formData)
    .subscribe({
      next: (response) => {
        this.expenseTransactions = [response, ...this.expenseTransactions];
        this.successMessage = "Transaction created successfully!";
        this.successTimeout = setTimeout(() => {
              this.successMessage = null;
            }, 3000);
      },
      error: (err) => {
        this.errorMessage = err.error || "An error occurred while creating the transaction.";
      },
      complete: () => {
        console.log("Transaction creation request completed");
        // Reset the form after successful submission
        if (this.transactionFormChild) {
          this.transactionFormChild.resetForm();
        }
      },
    });
  }
}
