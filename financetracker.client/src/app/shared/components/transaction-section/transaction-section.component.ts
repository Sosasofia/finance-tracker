import { Component, Input, ViewChild } from "@angular/core";
import { CommonModule } from "@angular/common";
import { MatTableModule } from "@angular/material/table";

import { TransactionFormComponent } from "../../../features/transactions/transaction-form/transaction-form.component";
import { LoadingComponent } from "../loading/loading.component";
import { TransactionService } from "../../../core/services/transaction.service";
import { Transaction } from "../../../models/transaction.model";

@Component({
  selector: "app-transaction-section",
  standalone: true,
  templateUrl: "./transaction-section.component.html",
  styleUrl: "./transaction-section.component.css",
  imports: [
    TransactionFormComponent,
    CommonModule,
    MatTableModule,
    LoadingComponent,
  ],
})
export class TransactionSectionComponent {
  @ViewChild(TransactionFormComponent)
  transactionFormChild!: TransactionFormComponent;

  transactions: Transaction[] = [];
  @Input() transactionType: "income" | "expense" = "expense";
  @Input() displayedColumns: string[] = [];
  loading = false;

  errorMessage: string | null = null;
  successMessage: string | null = null;
  private successTimeout: any;

  constructor(private transactionService: TransactionService) {}

  ngOnInit(): void {
    this.loadTransactions();
  }

  ngOnDestroy(): void {
    if (this.successTimeout) {
      clearTimeout(this.successTimeout);
    }
  }

  loadTransactions(): void {
    this.loading = true;
    this.transactionService.getTransactions().subscribe({
      next: (transactions) => {
        this.transactions = transactions.filter(
          (t) =>
            t.type.toLocaleLowerCase() ===
            this.transactionType.toLocaleLowerCase(),
        );
        this.loading = false;
      },
      error: (err) => {
        console.error("Error fetching income transactions", err);
        this.loading = false;
      },
    });
  }

  handleFormSubmit(formData: Transaction): void {
    this.errorMessage = null;
    this.successMessage = null;

    this.transactionService.createTransaction(formData).subscribe({
      next: (response) => {
        this.transactions = [response, ...this.transactions];
        this.successMessage = "Transaction created successfully!";
        this.successTimeout = setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.errorMessage =
          err.error || "An error occurred while creating the transaction.";
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
