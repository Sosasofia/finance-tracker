import { Component } from '@angular/core';

import { TransactionLayoutComponent } from '../transactions/coomponents/transaction-layout/transaction-layout.component';

@Component({
  selector: 'app-expense',
  standalone: true,
  templateUrl: './expense.component.html',
  styleUrl: './expense.component.css',
  imports: [TransactionLayoutComponent],
})
export class ExpenseComponent {
  transactionType: 'expense' | 'income' = 'expense';
  displayedColumns: string[] = ['name', 'date', 'amount', 'actions'];
}
