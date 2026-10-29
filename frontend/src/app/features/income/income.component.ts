import { Component } from '@angular/core';

import { TransactionType } from '../../shared/models/transaction.model';
import { TransactionLayoutComponent } from '../transactions/coomponents/transaction-layout/transaction-layout.component';

@Component({
  selector: 'app-income',
  standalone: true,
  templateUrl: './income.component.html',
  styleUrl: './income.component.css',
  imports: [TransactionLayoutComponent],
})
export class IncomeComponent {
  transactionType = TransactionType.Income;
}
