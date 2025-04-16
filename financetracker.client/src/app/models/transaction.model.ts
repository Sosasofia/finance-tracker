export interface Installment {
  id: string;
  installmentNumber: number;
  amount: number;
  dueDate: Date;
  paymentDate: Date;
  isPaid: boolean;
}
export enum TransactionType {
  Income = "Income",
  Expense = "Expense",
  //Transfer = 'Transfer'
}

export interface Transaction {
  id: string;
  amount: number;
  bussinessName: string;
  description: string;
  date: string;
  notes: string;
  receiptUrl: string;
  type: TransactionType;
  categoryId: string;
  paymentMethodId: string;
  userId: string;
  installments: Installment[];
}
