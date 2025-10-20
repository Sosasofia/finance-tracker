import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { CommonModule } from "@angular/common";
import { MatTableModule } from "@angular/material/table";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar, MatSnackBarModule } from "@angular/material/snack-bar";

import { TransactionFormComponent } from "../../../features/transactions/transaction-form/transaction-form.component";
import { LoadingComponent } from "../loading/loading.component";
import { TransactionService } from "../../../core/services/transaction.service";
import {
  Transaction,
  TransactionType,
} from "../../../models/transaction.model";
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from "../confirm-dialog/confirm-dialog.component";
import {
  EditTransactionDialogComponent,
  EditTransactionDialogData,
} from "../edit-transaction-dialog/edit-transaction-dialog.component";

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
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
})
export class TransactionSectionComponent implements OnInit {
  @ViewChild(TransactionFormComponent)
  transactionFormChild!: TransactionFormComponent;
  
  @Input() transactionType: "income" | "expense" = "expense";
  @Input() displayedColumns: string[] = [];

  transactions: Transaction[] = []; 
  loading = false;

  public TransactionType = TransactionType;

  // These messages are specifically for the ADD FORM submission outcome.
  errorMessage: string | null = null;
  successMessage: string | null = null;
  private successTimeout: any;

  constructor(
    private transactionService: TransactionService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
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
        }, 2000); // Message disappears after 2 seconds
      },
      error: (err) => {
        console.error("Error creating transaction:", err);
        this.errorMessage =
          err.error?.message ||
          "An error occurred while creating the transaction.";
        this.successTimeout = setTimeout(() => {
          this.errorMessage = null;
        }, 3000);
      },
    });
  }

  editTransaction(transaction: Transaction): void {
    this.errorMessage = null;
    this.successMessage = null;
    clearTimeout(this.successTimeout);

    const dialogData: EditTransactionDialogData = {
      transaction,
      transactionType: this.transactionType.toLowerCase() as
        | "income"
        | "expense",
      confirmButtonText: "Save Changes",
    };

    const dialogRef = this.dialog.open(EditTransactionDialogComponent, {
      width: "1600px",
      height: "auto",
      data: dialogData,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result?.success && result.updatedTransaction) {
        console.log(result.message);
        this.snackBar.open(result.message, "Close", { duration: 3000 });
        // Update the transaction in the local list
        const updatedTrans = result.updatedTransaction;
        const index = this.transactions.findIndex(
          (t) => t.id === updatedTrans.id,
        );
        if (index !== -1) {
          this.transactions[index] = updatedTrans;
          this.transactions = [...this.transactions];
        }
      } else if (result?.message) {
        this.snackBar.open(result.message, "Close", { duration: 3000 });
      }
    });
  }

  // This method is called when the user clicks the "Delete" button for a transaction
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
