import { TestBed } from "@angular/core/testing";
import {
  HttpClientTestingModule,
  HttpTestingController,
} from "@angular/common/http/testing";

import { TransactionService } from "./transaction.service";
import { Transaction, TransactionType } from "../../models/transaction.model";
import { environment } from "../../../environments/environment";

describe("TransactionService", () => {
  let service: TransactionService;
  let httpMock: HttpTestingController;

  const apiUrl = environment.apiUrl;

  const mockTransactions: Transaction[] = [
    {
      name: "Groceries",
      date: "2024-01-01",
      description: "Supermarket",
      amount: 100,
      notes: "",
      type: TransactionType.Expense,
      isCreditCardPurchase: false,
      categoryId: "",
      paymentMethodId: "",
      userId: "",
      isReimbursement: false,
    },
    {
      name: "Bonus",
      date: "2025-05-04",
      description: "",
      amount: 200,
      notes: "",
      type: TransactionType.Income,
      isCreditCardPurchase: false,
      categoryId: "",
      paymentMethodId: "",
      userId: "",
      isReimbursement: false,
    },
    {
      name: "Salary",
      date: "2024-05-03",
      description: "",
      amount: 200,
      notes: "",
      type: TransactionType.Income,
      isCreditCardPurchase: false,
      categoryId: "",
      paymentMethodId: "",
      userId: "",
      isReimbursement: false,
    },
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TransactionService],
    });

    service = TestBed.inject(TransactionService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  it("should fetch and sort transactions by date descending", () => {
    service.getTransactions().subscribe((transactions) => {
      expect(transactions.length).toBe(3);
      expect(transactions[0].date).toBe("2025-05-04");
      expect(transactions[1].date).toBe("2024-05-03");
      expect(transactions[2].date).toBe("2024-01-01");
    });

    const req = httpMock.expectOne(`${apiUrl}/transaction`);
    expect(req.request.method).toBe("GET");
    req.flush(mockTransactions);
  });

  it("should fetch a transaction by ID", () => {
    const transactionId = 1;
    const mockTransaction = mockTransactions[0];

    service.getTransactionById(transactionId).subscribe((transaction) => {
      expect(transaction).toEqual(mockTransaction);
    });

    const req = httpMock.expectOne(`${apiUrl}/transaction/${transactionId}`);
    expect(req.request.method).toBe("GET");
    req.flush(mockTransaction);
  });

  it("should create a transaction", () => {
    const newTransaction = mockTransactions[1];

    service.createTransaction(newTransaction).subscribe((transaction) => {
      expect(transaction).toEqual(newTransaction);
    });

    const req = httpMock.expectOne(`${apiUrl}/transaction`);
    expect(req.request.method).toBe("POST");
    expect(req.request.body).toEqual(newTransaction);
    req.flush(newTransaction);
  });

  it("should get payment methods", () => {
    const mockMethods = ["Cash", "Credit Card"];

    service.getPaymentMethods().subscribe((methods) => {
      expect(methods).toEqual(mockMethods);
    });

    const req = httpMock.expectOne(`${apiUrl}/payment-method`);
    expect(req.request.method).toBe("GET");
    req.flush(mockMethods);
  });

  it("should get and reverse categories", () => {
    const mockCategories = ["Food", "Transport"];

    service.getCategories().subscribe((categories) => {
      expect(categories).toEqual(mockCategories);
    });

    const req = httpMock.expectOne(`${apiUrl}/category`);
    expect(req.request.method).toBe("GET");
    req.flush(mockCategories);
  });
});
