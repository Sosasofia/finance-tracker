import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Transaction } from "../models/transaction.model";
import { environment } from "../../environments/environment";

@Injectable({
  providedIn: "root",
})
export class TransactionService {
  private apiUrl = environment.apiUrl + "/transaction";

  constructor(private http: HttpClient) {}

  getTransactions(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(this.apiUrl);
  }
}
