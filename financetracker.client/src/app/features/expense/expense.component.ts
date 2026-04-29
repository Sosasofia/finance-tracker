import { Component } from '@angular/core';

import { TransactionType } from '../../shared/models/transaction.model';
import { TransactionLayoutComponent } from '../transactions/coomponents/transaction-layout/transaction-layout.component';

@Component({
  selector: 'app-expense',
  standalone: true,
  templateUrl: './expense.component.html',
  styleUrl: './expense.component.css',
  imports: [TransactionLayoutComponent],
})
export class ExpenseComponent {
  transactionType = TransactionType.Expense;
}
