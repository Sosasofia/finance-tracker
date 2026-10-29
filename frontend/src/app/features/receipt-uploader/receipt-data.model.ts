export interface ExtractedReceiptData {
  merchantName: string | null;
  merchantNameConfidence: number | null;
  amount: number | null;
  totalAmountConfidence: number | null;
  transactionDate: string | null;
  transactionDateConfidence: number | null;
  lineItems: string[];
  categoryId: string | null;
  categoryConfidence: number | null;
  paymentMethodId: string | null;
  paymentMethodConfidence: number | null;
  rawCategoryText: string | null;
  rawPaymentMethodText: string | null;
}
