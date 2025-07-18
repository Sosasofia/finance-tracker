import { Component, Input, ViewChild } from "@angular/core";
import { CommonModule } from "@angular/common";
import { MatTableModule } from "@angular/material/table";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatDialog } from "@angular/material/dialog";

import { TransactionFormComponent } from "../../../features/transactions/transaction-form/transaction-form.component";
import { LoadingComponent } from "../loading/loading.component";
import { TransactionService } from "../../../core/services/transaction.service";
import { Transaction } from "../../../models/transaction.model";
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from "../confirm-dialog/confirm-dialog.component";


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
    MatIconModule,
    MatButtonModule,
    ConfirmDialogComponent,
    MatProgressSpinnerModule,
  ],
})
export class TransactionSectionComponent {
  @ViewChild(TransactionFormComponent)
  transactionFormChild!: TransactionFormComponent;

  transactions: Transaction[] = [];
  @Input() transactionType: "income" | "expense" = "expense";
  @Input() displayedColumns: string[] = [];
  @Input() loading = false;

  // These messages are specifically for the ADD FORM submission outcome.
  errorMessage: string | null = null;
  successMessage: string | null = null;
  private successTimeout: any;

  constructor(
    private transactionService: TransactionService,
    private dialog: MatDialog,
  ) {}

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
        // Reset the form after successful submission
        this.successTimeout = setTimeout(() => {
          this.successMessage = null;
        }, 2000);  // Message disappears after 2 seconds
      },
      error: (err) => {
        console.error("Error creating transaction:", err);
        this.errorMessage =
          err.error?.message || "An error occurred while creating the transaction."; 
        this.successTimeout = setTimeout(() => { 
            this.errorMessage = null;
        }, 3000); 
      },
    });
  }

  editTransaction(transaction: Transaction): void {
    console.log("Editing transaction:", transaction);
  }

  deleteTransaction(transaction: Transaction): void {
    this.errorMessage = null;
    this.successMessage = null;
    clearTimeout(this.successTimeout);

    const dialogData: ConfirmDialogData = {
      title: "Confirm Deletion",
      message: `Are you sure you want to delete the transaction "${transaction.name}"? This action cannot be undone.`,
      confirmButtonText: "Delete",
      cancelButtonText: "Cancel",
      transactionIdToDelete: transaction.id,
    };

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: "400px",
      data: dialogData,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result && result.success) {
        this.transactions = this.transactions.filter(
          (t) => t.id !== transaction.id,
        );
      } else {
        console.log("Delete dialog dismissed or failed:", result?.message);
      }
    });
  }
}
