import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Transaction } from '../../models/transaction.model';

@Injectable({
  providedIn: 'root',
})
export class TransactionService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getTransactions(): Observable<Transaction[]> {
    return this.http
      .get<Transaction[]>(`${this.apiUrl}/transaction`)
      .pipe(map((transactions) => transactions.sort((a, b) => b.date.localeCompare(a.date))));
  }

  getTransactionById(id: number): Observable<Transaction> {
    return this.http.get<Transaction>(`${this.apiUrl}/transaction/${id}`);
  }

  createTransaction(transaction: Transaction): Observable<Transaction> {
    return this.http.post<Transaction>(`${this.apiUrl}/transaction`, transaction);
  }

  updateTransaction(transactionId: string, transaction: Transaction): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.apiUrl}/transaction/${transactionId}`, transaction);
  }

  deleteTransaction(transactionId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/transaction/${transactionId}`);
  }

  getPaymentMethods(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/payment-method`);
  }

  getCategories(): Observable<any[]> {
    return this.http
      .get<any[]>(`${this.apiUrl}/category`)
      .pipe(map((categories) => categories.reverse()));
  }
}
