export interface ExtractedReceiptData {
  merchantName: string | null;
  totalAmount: number | null;
  transactionDate: string | null;
  lineItems: string[];
}
