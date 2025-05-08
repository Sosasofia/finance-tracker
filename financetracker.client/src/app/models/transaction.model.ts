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
  id?: string;
  amount: number;
  name: string;
  description?: string;
  date: string;
  notes: string;
  receiptUrl?: string;
  type: TransactionType;
  isCreditCardPurchase: boolean;
  categoryId: string;
  paymentMethodId: string;
  userId: string;
  isReimbursement: boolean;
  installments?: Installment[];
  installment?: {
    number: number;
    interest: number;
  };
  reimbursement?: {
    amount: number;
    date: string;
    reason: string;
  };
}
