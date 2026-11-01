import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { ReceiptLibraryComponent } from '../receipt-uploader/receipt-library.component';
import { ExtractedReceiptData } from '../receipt-uploader/receipt-data.model';
import { AddTransactionDialogComponent } from '../transactions/coomponents/add-transaction-dialog/add-transaction-dialog.component';
import { TransactionType } from '../../shared/models/transaction.model';

@Component({
  selector: 'app-receipts',
  standalone: true,
  imports: [ReceiptLibraryComponent, MatIconModule],
  template: `
    <section class="receipts-page">
      <header class="receipts-heading">
        <div class="heading-icon"><mat-icon>photo_camera</mat-icon></div>
        <div>
          <p class="eyebrow">Your documents</p>
          <h1>Receipts</h1>
          <p class="heading-copy">Keep every receipt close, organized and ready to become a transaction.</p>
        </div>
      </header>
      <div class="receipts-card">
        <app-receipt-library (processed)="openExpenseForm($event)"></app-receipt-library>
      </div>
    </section>
  `,
  styleUrl: './receipts.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReceiptsComponent {
  private readonly dialog = inject(MatDialog);

  openExpenseForm(data: ExtractedReceiptData): void {
    this.dialog.open(AddTransactionDialogComponent, {
      width: '95%',
      maxWidth: '500px',
      maxHeight: '90vh',
      panelClass: 'rounded-dialog',
      data: { transactionType: TransactionType.Expense, scannedReceiptData: data },
    });
  }
}
