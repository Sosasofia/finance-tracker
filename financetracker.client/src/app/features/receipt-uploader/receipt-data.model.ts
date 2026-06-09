export interface ExtractedReceiptData {
  merchantName: string | null;
  merchantNameConfidence: number | null;
  totalAmount: number | null;
  totalAmountConfidence: number | null;
  transactionDate: string | null;
  transactionDateConfidence: number | null;
  lineItems: string[];
}
