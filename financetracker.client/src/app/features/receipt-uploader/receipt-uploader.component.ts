import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ReceiptScannerService } from './receipt-scanner.service';

@Component({
  selector: 'app-receipt-uploader',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatDialogModule, MatProgressSpinnerModule],
  templateUrl: './receipt-uploader.component.html',
  styleUrls: ['./receipt-uploader.component.css'],
})
export class ReceiptUploaderComponent {
  private receiptScannerService = inject(ReceiptScannerService);
  private dialogRef = inject(MatDialogRef<ReceiptUploaderComponent>);

  selectedFile: File | null = null;
  isScanning = false;
  errorMessage: string | null = null;

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.errorMessage = null;
    }
  }

  uploadAndScan(): void {
    if (!this.selectedFile) return;

    this.isScanning = true;
    this.errorMessage = null;

    this.receiptScannerService.scanReceipt(this.selectedFile).subscribe({
      next: (data) => {
        this.isScanning = false;
        this.dialogRef.close(data);
      },
      error: (err) => {
        console.error('Scanning failed:', err);
        this.errorMessage = 'Failed to extract receipt data. Please try again.';
        this.isScanning = false;
      },
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
