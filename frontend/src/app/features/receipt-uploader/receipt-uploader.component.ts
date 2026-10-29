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

  async uploadAndScan(): Promise<void> {
    if (!this.selectedFile) return;

    this.isScanning = true;
    this.errorMessage = null;

    try {
      const compressedFile = await this.compressImage(this.selectedFile);

      this.receiptScannerService.scanReceipt(compressedFile).subscribe({
        next: (data) => {
          this.isScanning = false;

          if (!data || (!data.amount && !data.merchantName)) {
            this.errorMessage =
              "We couldn't read this receipt clearly. Please ensure the image is well-lit and try again.";
            return;
          }

          this.dialogRef.close(data);
        },
        error: (err) => {
          console.error('Scanning failed:', err);
          this.errorMessage = 'Failed to extract receipt data. Please try again.';
          this.isScanning = false;
        },
      });
    } catch (error) {
      console.error('Image compression failed:', error);
      this.errorMessage = 'Failed to process the image. Please try a different file.';
      this.isScanning = false;
    }
  }

  /**
   * Compresses images locally using an HTML5 Canvas before sending them to the API.
   * Significantly reduces 10MB+ camera images down to ~1MB.
   */
  private compressImage(file: File): Promise<File> {
    return new Promise((resolve, reject) => {
      if (!file.type.startsWith('image/')) {
        resolve(file);
        return;
      }

      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = (event) => {
        const img = new Image();
        img.src = event.target?.result as string;

        img.onload = () => {
          const canvas = document.createElement('canvas');
          let width = img.width;
          let height = img.height;

          const MAX_WIDTH = 1920;
          const MAX_HEIGHT = 1920;

          if (width > height) {
            if (width > MAX_WIDTH) {
              height *= MAX_WIDTH / width;
              width = MAX_WIDTH;
            }
          } else {
            if (height > MAX_HEIGHT) {
              width *= MAX_HEIGHT / height;
              height = MAX_HEIGHT;
            }
          }

          canvas.width = width;
          canvas.height = height;
          const ctx = canvas.getContext('2d');

          if (!ctx) {
            resolve(file);
            return;
          }

          ctx.drawImage(img, 0, 0, width, height);

          canvas.toBlob(
            (blob) => {
              if (blob) {
                const newFileName = file.name.replace(/\.[^/.]+$/, '') + '.jpg';
                const compressedFile = new File([blob], newFileName, {
                  type: 'image/jpeg',
                  lastModified: Date.now(),
                });
                resolve(compressedFile);
              } else {
                resolve(file);
              }
            },
            'image/jpeg',
            0.75
          );
        };
        img.onerror = (error) => reject(error);
      };
      reader.onerror = (error) => reject(error);
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
