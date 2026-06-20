import { Component, inject, output, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { TransactionService } from '../../../../core/services/transaction.service';
import { Transaction, TransactionType } from '../../../../shared/models/transaction.model';
import { TransactionFormComponent } from '../transaction-form/transaction-form.component';

export interface EditTransactionDialogData {
  transaction: Transaction;
  transactionType: TransactionType;
  confirmButtonText: string;
}

@Component({
  selector: 'app-edit-transaction-dialog',
  standalone: true,
  templateUrl: './edit-transaction-dialog.component.html',
  styleUrls: ['./edit-transaction-dialog.component.css'],
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    TransactionFormComponent,
    MatProgressSpinnerModule,
  ],
})
export class EditTransactionDialogComponent {
  transactionFormChild = viewChild(TransactionFormComponent);

  isLoading = false;
  apiSuccess: boolean | null = null;
  apiMessage: string | null = null;

  private transactionService = inject(TransactionService);
  public dialogRef = inject<
    MatDialogRef<
      EditTransactionDialogComponent,
      {
        success: boolean;
        action?: string;
        id?: string;
        updatedTransaction?: Transaction;
        message?: string;
      }
    >
  >(MatDialogRef);
  public data = inject<EditTransactionDialogData>(MAT_DIALOG_DATA);

  triggerFormSave(): void {
    this.transactionFormChild()?.triggerSubmit();
  }

  onFormSubmitted(updatedFormData: Transaction): void {
    this.isLoading = true;
    this.apiSuccess = null;
    this.apiMessage = null;

    const completeTransactionPayload: Transaction = {
      ...this.data.transaction,
      ...updatedFormData,
    };

    this.transactionService
      .updateTransaction(this.data.transaction.id!, completeTransactionPayload)
      .subscribe({
        next: (response) => {
          this.isLoading = false;
          this.apiSuccess = true;
          this.apiMessage = 'Transaction updated successfully!';

          this.dialogRef.close({
            success: true,
            updatedTransaction: response,
            message: this.apiMessage,
          });
        },
        error: (err) => {
          this.isLoading = false;
          this.apiSuccess = false;
          this.apiMessage = err.error?.detail || err.error || 'Failed to update transaction.';
        },
      });
  }

  onCancel(): void {
    this.dialogRef.close({ success: false });
  }
}
