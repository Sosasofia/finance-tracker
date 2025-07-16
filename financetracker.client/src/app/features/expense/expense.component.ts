import { Component } from "@angular/core";

import { TransactionSectionComponent } from "../../shared/components/transaction-section/transaction-section.component";

@Component({
  selector: "app-expense",
  standalone: true,
  templateUrl: "./expense.component.html",
  styleUrl: "./expense.component.css",
  imports: [TransactionSectionComponent],
})
export class ExpenseComponent {
  transactionType: "expense" = "expense";
  displayedColumns: string[] = ["name", "date", "amount"];
}
