import { Component } from "@angular/core";

import { TransactionSectionComponent } from "../../shared/components/transaction-section/transaction-section.component";

@Component({
  selector: "app-income",
  standalone: true,
  templateUrl: "./income.component.html",
  styleUrl: "./income.component.css",
  imports: [TransactionSectionComponent],
})
export class IncomeComponent {
  transactionType: "income" = "income"; // Fixed to 'income'
  displayedColumns: string[] = ["name", "date", "amount"];
}
