import { Component, inject, signal, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogModule,
  MatDialogRef,
} from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

import { TransactionService } from '../../../../core/services/transaction.service';
import { Transaction } from '../../../../shared/models/transaction.model';
import { ExtractedReceiptData } from '../../../receipt-uploader/receipt-data.model';
import { ReceiptUploaderComponent } from '../../../receipt-uploader/receipt-uploader.component';
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
  ],
})
export class AddTransactionDialogComponent {
  transactionFormChild = viewChild(TransactionFormComponent);

  isLoading = false;
  apiMessage: string | null = null;

  scannedReceiptData = signal<ExtractedReceiptData | null>(null);

  private transactionService = inject(TransactionService);
  public dialogRef = inject(MatDialogRef<AddTransactionDialogComponent>);
  public data = inject<{ transactionType: Transaction['type'] }>(MAT_DIALOG_DATA);

  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  openReceiptScanner(): void {
    const scannerRef = this.dialog.open(ReceiptUploaderComponent, {
      width: '95%',
      maxWidth: '450px',
      panelClass: 'rounded-dialog',
      disableClose: true,
    });

    scannerRef.afterClosed().subscribe((result: ExtractedReceiptData | undefined) => {
      if (result) {
        this.scannedReceiptData.set(result);
        this.snackBar.open('Receipt scanned! Please review the details.', 'Close', {
          duration: 3000,
        });
      }
    });
  }

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

  triggerFormSave(): void {
    this.transactionFormChild()?.triggerSubmit();
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
