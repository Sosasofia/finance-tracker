import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ExtractedReceiptData } from './receipt-data.model';

export interface ReceiptSecureUrlResponse {
  url: string;
  expiresAt: string;
}

@Injectable({
  providedIn: 'root',
})
export class ReceiptScannerService {
  private apiUrl = environment.apiUrl;
  private readonly scanUrl = `${this.apiUrl}/receipts/scan`;

  constructor(private http: HttpClient) {}

  scanReceipt(file: File): Observable<ExtractedReceiptData> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post<ExtractedReceiptData>(this.scanUrl, formData);
  }

  getReceiptUrl(receiptId: string): Observable<ReceiptSecureUrlResponse> {
    return this.http.get<ReceiptSecureUrlResponse>(
      `${this.apiUrl}/receipts/${encodeURIComponent(receiptId)}/url`
    );
  }

  downloadReceipt(receiptId: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/receipts/${encodeURIComponent(receiptId)}`, {
      responseType: 'blob',
    });
  }

}
