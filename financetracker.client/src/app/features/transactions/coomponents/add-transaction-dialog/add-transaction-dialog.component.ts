import { DatePipe } from '@angular/common';
import { Component, inject, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { TransactionService } from '../../../../core/services/transaction.service';
import { Transaction } from '../../../../shared/models/transaction.model';
import { TransactionFormComponent } from '../transaction-form/transaction-form.component';

@Component({
  selector: 'app-add-transaction-dialog',
  standalone: true,
  templateUrl: './add-transaction-dialog.component.html',
  styleUrls: ['./add-transaction-dialog.component.css'],
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    TransactionFormComponent,
    MatProgressSpinnerModule,
    DatePipe,
  ],
})
export class AddTransactionDialogComponent {
  transactionFormChild = viewChild(TransactionFormComponent);

  isLoading = false;
  apiMessage: string | null = null;

  private transactionService = inject(TransactionService);
  public dialogRef = inject(MatDialogRef<AddTransactionDialogComponent>);
  public data = inject<{ transactionType: Transaction['type'] }>(MAT_DIALOG_DATA);

  onFormSubmitted(newTransactionData: Transaction): void {
    this.isLoading = true;
    this.apiMessage = null;

    newTransactionData.type = this.data.transactionType;

    this.transactionService.createTransaction(newTransactionData).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.dialogRef.close({ success: true, newTransaction: response });
      },
      error: (err) => {
        this.isLoading = false;
        this.apiMessage = err.error?.title || err.error || 'Failed to create transaction.';
      },
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
