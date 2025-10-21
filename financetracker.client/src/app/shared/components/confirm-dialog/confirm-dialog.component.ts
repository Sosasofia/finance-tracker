import { Component, Inject, OnDestroy } from "@angular/core";
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogModule,
} from "@angular/material/dialog";
import { MatButtonModule } from "@angular/material/button";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { CommonModule } from "@angular/common";
import { MatIcon } from "@angular/material/icon";

import { TransactionService } from "../../../core/services/transaction.service";

export interface ConfirmDialogData {
  title: string;
  message: string;
  confirmButtonText: string;
  cancelButtonText: string;
  transactionIdToDelete?: string;
}

@Component({
  selector: "app-confirm-dialog",
  standalone: true,
  templateUrl: "./confirm-dialog.component.html",
  styleUrls: ["./confirm-dialog.component.css"],
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    CommonModule,
    MatIcon,
  ],
})
export class ConfirmDialogComponent implements OnDestroy {
  isLoading = false;
  apiSuccess: boolean | null = null;
  apiMessage: string | null = null;

  private closeTimeout: any;
  constructor(
    public dialogRef: MatDialogRef<
      ConfirmDialogComponent,
      { success: boolean; message?: string }
    >,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogData,
    private transactionService: TransactionService,
  ) {}

  onConfirm(): void {
    this.isLoading = true;
    this.apiSuccess = null;
    this.apiMessage = null;

    // Call the delete service directly from the dialog
    this.transactionService
      .deleteTransaction(this.data.transactionIdToDelete!)
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.apiSuccess = true;
          this.apiMessage = `Transaction deleted successfully!`;

          this.closeTimeout = setTimeout(() => {
            this.dialogRef.close({ success: true, message: this.apiMessage! });
          }, 1500);
        },
        error: (err) => {
          this.isLoading = false;
          this.apiSuccess = false;
          this.apiMessage =
            err.error?.message || "An error occurred during deletion.";
          console.error("Error deleting transaction in dialog:", err);

          // Auto-close dialog after a short delay on error too
          this.closeTimeout = setTimeout(() => {
            this.dialogRef.close({ success: false, message: this.apiMessage! });
          }, 2500);
        },
      });
  }

  onCancel(): void {
    this.dialogRef.close({
      success: false,
      message: "Deletion cancelled by user.",
    });
  }

  ngOnDestroy(): void {
    if (this.closeTimeout) {
      clearTimeout(this.closeTimeout);
    }
  }
}
