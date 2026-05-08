import { BreakpointObserver } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { Component, Input, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { firstValueFrom } from 'rxjs';
import { map } from 'rxjs/operators';

import { TransactionService } from '../../../../core/services/transaction.service';
import { CategoryFilterComponent } from '../../../../shared/components/category-filter/category-filter.component';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';
import { Transaction, TransactionType } from '../../../../shared/models/transaction.model';
import { TransactionStore } from '../../state/transaction.store';
import { AddTransactionDialogComponent } from '../add-transaction-dialog/add-transaction-dialog.component';
import { EditTransactionDialogComponent } from '../edit-transaction-dialog/edit-transaction-dialog.component';
import { TransactionFormComponent } from '../transaction-form/transaction-form.component';

@Component({
  selector: 'app-transaction-layout',
  standalone: true,
  templateUrl: './transaction-layout.component.html',
  styleUrl: './transaction-layout.component.css',
  imports: [
    TransactionFormComponent,
    CommonModule,
    MatTableModule,
    LoadingComponent,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    CategoryFilterComponent,
  ],
})
export class TransactionLayoutComponent {
  public TransactionTypeEnum = TransactionType;

  @Input() transactionType: TransactionType = TransactionType.Expense;

  readonly store = inject(TransactionStore);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private transactionService = inject(TransactionService);
  private breakpointObserver = inject(BreakpointObserver);

  activeCategory = signal<string>('All');

  uniqueCategories = computed(() => {
    const txs = this.store
      .transactions()
      .filter((t) => t.type.toLowerCase() === this.transactionType.toLowerCase());

    const cats = new Set<string>();
    for (const t of txs) {
      cats.add((t as any)?.category?.name ?? t.categoryId ?? 'Uncategorized');
    }
    return ['All', ...Array.from(cats)];
  });

  filteredTransactions = computed(() => {
    const category = this.activeCategory();

    return this.store.transactions().filter((t) => {
      const isRightType = t.type.toLowerCase() === this.transactionType.toLowerCase();

      const catName = (t as any)?.category?.name ?? t.categoryId ?? 'Uncategorized';
      const isRightCategory = category === 'All' || catName === category;

      return isRightType && isRightCategory;
    });
  });

  isMobile = toSignal(
    this.breakpointObserver.observe('(max-width: 900px)').pipe(map((res) => res.matches)),
    { initialValue: false }
  );

  selectedTransaction = signal<Transaction | null>(null);
  expandedTransactionId = signal<string | null>(null);

  openAddTransaction() {
    const dialogRef = this.dialog.open(AddTransactionDialogComponent, {
      width: '90%',
      maxWidth: '500px',
      panelClass: 'rounded-dialog',
      data: { transactionType: this.transactionType },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result?.success) {
        this.store.loadTransactions();
        this.snackBar.open('Transaction created successfully!', 'Close', { duration: 3000 });
      }
    });
  }

  editTransaction(transaction: Transaction): void {
    if (this.isMobile()) {
      this.expandedTransactionId.update((id) => (id === transaction.id ? null : transaction.id!));
      return;
    }

    this.selectedTransaction.set(transaction);
  }

  deleteTransaction(transactionId: string): void {
    if (!transactionId) {
      return;
    }

    const dialogData: ConfirmDialogData = {
      title: 'Confirm Deletion',
      message: 'Are you sure you want to delete this transaction? This action cannot be undone.',
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel',
    };

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: dialogData,
      panelClass: 'rounded-dialog',
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(async (isConfirmed: boolean) => {
      if (isConfirmed) {
        try {
          await this.store.deleteTransaction(transactionId);
          if (this.selectedTransaction()?.id === transactionId) {
            this.selectedTransaction.set(null);
          }
          this.snackBar.open('Transaction deleted successfully!', 'Dismiss', { duration: 3000 });
        } catch (error) {
          this.snackBar.open('Error: Could not delete transaction.', 'Dismiss', { duration: 5000 });
        }
      }
    });
  }

  openEditTransactionDialog(transaction: Transaction): void {
    const dialogRef = this.dialog.open(EditTransactionDialogComponent, {
      width: '90%',
      maxWidth: '600px',
      maxHeight: '90vh',
      panelClass: 'rounded-dialog',
      data: {
        transaction,
        transactionType: this.transactionType,
        confirmButtonText: 'Save Changes',
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result?.success) {
        this.store.loadTransactions();
        this.snackBar.open(result.message ?? 'Transaction updated successfully!', 'Close', {
          duration: 3000,
        });
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

      await this.store.loadTransactions();

      this.selectedTransaction.set(null);
    } catch (err: any) {
      console.error('Error saving transaction:', err);
      const msg = err.error?.message || 'An error occurred while saving the transaction.';
      this.snackBar.open(msg, 'Close', { duration: 5000 });
    }
  }
}
