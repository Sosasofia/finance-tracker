import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExtractedReceiptData } from './receipt-data.model';

@Injectable({
  providedIn: 'root',
})
export class ReceiptScannerService {
  private readonly apiUrl = '/api/receipts/scan';

  constructor(private http: HttpClient) {}

  scanReceipt(file: File): Observable<ExtractedReceiptData> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post<ExtractedReceiptData>(this.apiUrl, formData);
  }
}
