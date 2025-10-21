import { Component, Inject, ViewChild, AfterViewInit } from "@angular/core";
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogModule,
} from "@angular/material/dialog";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { CommonModule } from "@angular/common";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

import { TransactionFormComponent } from "../../../features/transactions/transaction-form/transaction-form.component";
import { Transaction } from "../../../models/transaction.model";
import { TransactionService } from "../../../core/services/transaction.service";

export interface EditTransactionDialogData {
  transaction: Transaction; // The transaction object to be edited
  transactionType: "income" | "expense";
  confirmButtonText: string;
}

@Component({
  selector: "app-edit-transaction-dialog",
  standalone: true,
  templateUrl: "./edit-transaction-dialog.component.html",
  styleUrls: ["./edit-transaction-dialog.component.css"],
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    CommonModule,
    TransactionFormComponent,
    MatProgressSpinnerModule,
  ],
})
export class EditTransactionDialogComponent implements AfterViewInit {
  @ViewChild(TransactionFormComponent)
  transactionFormChild!: TransactionFormComponent;

  isLoading = false;
  apiSuccess: boolean | null = null;
  apiMessage: string | null = null;

  private closeTimeout: ReturnType<typeof setTimeout> | null = null;

  constructor(
    private transactionService: TransactionService,
    public dialogRef: MatDialogRef<
      EditTransactionDialogComponent,
      { success: boolean; updatedTransaction?: Transaction; message?: string }
    >,
    @Inject(MAT_DIALOG_DATA) public data: EditTransactionDialogData,
  ) {}

  ngAfterViewInit(): void {
    if (this.transactionFormChild && this.data.transaction) {
      this.transactionFormChild.setFormValues(this.data.transaction);
    }
  }

  onFormSubmitted(updatedFormData: Transaction): void {
    this.isLoading = true;
    this.apiSuccess = null;
    this.apiMessage = null;

    this.transactionService
      .updateTransaction(this.data.transaction.id!, updatedFormData)
      .subscribe({
        next: (response) => {
          this.isLoading = false;
          this.apiSuccess = true;
          this.apiMessage = "Transaction updated successfully!";

          this.transactionFormChild.resetForm();
          this.dialogRef.close({
            success: true,
            updatedTransaction: response,
            message: this.apiMessage,
          });
        },
        error: (err) => {
          this.isLoading = false;
          this.apiSuccess = false;
          this.apiMessage = err.error || "Failed to update transaction.";
        },
      });
  }

  onCancel(): void {
    this.dialogRef.close({
      success: false,
      message: "Transaction edit cancelled.",
    });
  }
}
