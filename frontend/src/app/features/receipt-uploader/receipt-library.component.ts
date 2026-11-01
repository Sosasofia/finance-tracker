import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, OnInit, Output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ReceiptListItem, ReceiptScannerService } from './receipt-scanner.service';
import { ExtractedReceiptData } from './receipt-data.model';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-receipt-library',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatDialogModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
  template: `
    <div class="receipt-library">
      <div class="receipt-upload">
        <label class="upload-dropzone">
          <mat-icon>cloud_upload</mat-icon>
          <span>Choose a receipt</span>
          <small>JPG, PNG or PDF up to 5 MB</small>
          <input type="file" accept=".jpg,.jpeg,.png,.pdf" (change)="selectFile($event)" />
        </label>
        @if (selectedFile()) {
          <div class="name-field">
            <mat-icon>edit</mat-icon>
            <input [(ngModel)]="displayName" placeholder="Receipt name" aria-label="Receipt name" />
          </div>
          <button
            mat-flat-button
            color="primary"
            (click)="upload()"
            [disabled]="loading() || !displayName.trim()"
          >
            Save receipt
          </button>
        }
      </div>
      @if (loading()) {
        <mat-spinner diameter="24"></mat-spinner>
      }
      @for (receipt of receipts(); track receipt.id) {
        <div class="receipt-row">
          <div class="file-icon" [class.pdf]="receipt.contentType === 'application/pdf'">
            <mat-icon>{{
              receipt.contentType === 'application/pdf' ? 'picture_as_pdf' : 'image'
            }}</mat-icon>
          </div>
          <div class="receipt-info">
            <strong>{{ receipt.displayName }}</strong>
            <small>{{ receipt.originalFileName }} · {{ receipt.uploadedAt | date }}</small>
          </div>
          <button mat-icon-button (click)="view(receipt)" aria-label="View receipt">
            <mat-icon>visibility</mat-icon>
          </button>
          <button mat-icon-button (click)="process(receipt)" aria-label="Process receipt">
            <mat-icon>receipt_long</mat-icon>
          </button>
          <button mat-icon-button (click)="remove(receipt)" aria-label="Delete receipt">
            <mat-icon>delete</mat-icon>
          </button>
        </div>
      } @empty {
        <p>No saved receipts yet.</p>
      }
    </div>
  `,
  styleUrl: './receipt-library.component.css',
})
export class ReceiptLibraryComponent implements OnInit {
  @Input() compact = false;
  @Output() processed = new EventEmitter<ExtractedReceiptData>();
  private readonly service = inject(ReceiptScannerService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  readonly receipts = signal<ReceiptListItem[]>([]);
  readonly selectedFile = signal<File | null>(null);
  readonly loading = signal(false);
  displayName = '';

  ngOnInit(): void {
    this.refresh();
  }

  selectFile(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0] ?? null;
    this.selectedFile.set(file);
    this.displayName = file?.name.replace(/\.[^/.]+$/, '') ?? '';
  }

  upload(): void {
    const file = this.selectedFile();
    if (!file) return;
    this.loading.set(true);
    this.service.uploadReceipt(file, this.displayName.trim()).subscribe({
      next: () => {
        this.selectedFile.set(null);
        this.displayName = '';
        this.refresh();
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Could not save receipt.', 'Close', { duration: 3000 });
      },
    });
  }

  view(receipt: ReceiptListItem): void {
    this.service.getReceiptUrl(receipt.id).subscribe({
      next: ({ url }) => window.open(url, '_blank', 'noopener,noreferrer'),
      error: () => this.snackBar.open('Could not open receipt.', 'Close', { duration: 3000 }),
    });
  }

  process(receipt: ReceiptListItem): void {
    this.loading.set(true);
    this.service.processReceipt(receipt.id).subscribe({
      next: (data) => {
        this.loading.set(false);
        this.processed.emit(data);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Could not process receipt.', 'Close', { duration: 3000 });
      },
    });
  }

  remove(receipt: ReceiptListItem): void {
    const dialogData: ConfirmDialogData = {
      title: 'Delete receipt?',
      message: `Are you sure you want to delete "${receipt.displayName}"? This action cannot be undone.`,
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel',
    };

    this.dialog
      .open(ConfirmDialogComponent, {
        width: '400px',
        data: dialogData,
        panelClass: 'rounded-dialog',
        disableClose: true,
      })
      .afterClosed()
      .subscribe((isConfirmed: boolean) => {
        if (!isConfirmed) {
          return;
        }

        this.loading.set(true);
        this.service.deleteReceipt(receipt.id).subscribe({
          next: () => {
            this.snackBar.open('Receipt deleted successfully.', 'Dismiss', { duration: 3000 });
            this.refresh();
          },
          error: () => {
            this.loading.set(false);
            this.snackBar.open('Could not delete receipt.', 'Close', { duration: 3000 });
          },
        });
      });
  }

  private refresh(): void {
    this.loading.set(true);
    this.service.listReceipts().subscribe({
      next: (receipts) => {
        this.receipts.set(receipts);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Could not load receipts.', 'Close', { duration: 3000 });
      },
    });
  }
}
