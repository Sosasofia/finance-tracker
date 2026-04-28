import { Component } from '@angular/core';

import { TransactionLayoutComponent } from '../../shared/components/transaction-layout/transaction-layout.component';

@Component({
  selector: 'app-income',
  standalone: true,
  templateUrl: './income.component.html',
  styleUrl: './income.component.css',
  imports: [TransactionLayoutComponent],
})
export class IncomeComponent {
  transactionType: 'income' | 'expense' = 'income';
  displayedColumns: string[] = ['name', 'date', 'amount', 'actions'];
}
