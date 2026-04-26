import { CommonModule } from '@angular/common';
import { Component, Input, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { firstValueFrom } from 'rxjs';

import { TransactionService } from '../../../core/services/transaction.service';
import { TransactionStore } from '../../../features/transactions/state/transaction.store';
import { TransactionFormComponent } from '../../../features/transactions/transaction-form/transaction-form.component';
import { Transaction } from '../../models/transaction.model';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '../confirm-dialog/confirm-dialog.component';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-transaction-section',
  standalone: true,
  templateUrl: './transaction-section.component.html',
  styleUrl: './transaction-section.component.css',
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
export class TransactionSectionComponent {
  @Input() transactionType: 'income' | 'expense' = 'expense';
  @Input() displayedColumns: string[] = [];

  readonly store = inject(TransactionStore);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private transactionService = inject(TransactionService);

  selectedTransaction = signal<Transaction | null>(null);

  filteredTransactions = computed(() => {
    return this.store
      .transactions()
      .filter((t) => t.type.toLowerCase() === this.transactionType.toLowerCase());
  });

  editTransaction(transaction: Transaction): void {
    this.selectedTransaction.set(transaction);
  }

  deleteTransaction(transactionId: string): void {
    const dialogData: ConfirmDialogData = {
      title: 'Confirm Deletion',
      message: 'Are you sure you want to delete this transaction? This action cannot be undone.',
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel',
    };

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: dialogData,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(async (isConfirmed: boolean) => {
      if (isConfirmed) {
        try {
          await this.store.deleteTransaction(transactionId);
          this.snackBar.open('Transaction deleted successfully!', 'Dismiss', { duration: 3000 });
        } catch (error) {
          this.snackBar.open('Error: Could not delete transaction.', 'Dismiss', { duration: 5000 });
        }
      }
    });
  }

  async handleFormSubmit(formData: Transaction): Promise<void> {
    try {
      const currentTx = this.selectedTransaction();

      if (currentTx && currentTx.id) {
        await firstValueFrom(this.transactionService.updateTransaction(currentTx.id, formData));
        this.snackBar.open('Transaction updated successfully!', 'Close', { duration: 3000 });
      } else {
        const response = await firstValueFrom(this.transactionService.createTransaction(formData));

        this.store.addTransactionLocal(response);
        this.snackBar.open('Transaction created successfully!', 'Close', { duration: 3000 });
      }

      if (currentTx) {
        await this.store.loadTransactions();
      }

      this.selectedTransaction.set(null);
    } catch (err: any) {
      console.error('Error saving transaction:', err);
      const msg = err.error?.message || 'An error occurred while saving the transaction.';
      this.snackBar.open(msg, 'Close', { duration: 5000 });
    }
  }
}
