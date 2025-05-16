import { ComponentFixture, fakeAsync, TestBed, tick } from "@angular/core/testing";

import { IncomeComponent } from "./income.component";
import { Transaction, TransactionType } from "../../models/transaction.model";
import { TransactionService } from "../../core/services/transaction.service";
import { of, throwError } from "rxjs";

describe("IncomeComponent", () => {
  let component: IncomeComponent;
  let fixture: ComponentFixture<IncomeComponent>;

  let mockTransactionService: jasmine.SpyObj<TransactionService>;

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

  beforeEach(async () => {
    mockTransactionService = jasmine.createSpyObj("TransactionService", [
      "getTransactions",
      "createTransaction",
    ]);

    await TestBed.configureTestingModule({
      declarations: [],
      imports: [IncomeComponent],
      providers: [
        { provide: TransactionService, useValue: mockTransactionService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(IncomeComponent);
    component = fixture.componentInstance;
    //fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  it("should load only income transactions on init", fakeAsync(() => {
    mockTransactionService.getTransactions.and.returnValue(of(mockTransactions));

    component.loadIncomeTransactions();
    tick();

    expect(mockTransactionService.getTransactions).toHaveBeenCalled();
    expect(component.incomeTransactions.every(t => t.type === TransactionType.Income)).toBeTrue();
    expect(component.incomeTransactions.length).toBe(2);
  }));

  it("should handle form submit and add new transaction to list", () => {
    const newTransaction: Transaction = {
      name: "Salary",
      date: "2024-05-03",
      description: "",
      amount: 150,
      notes: "",
      type: TransactionType.Income,
      isCreditCardPurchase: false,
      categoryId: "",
      paymentMethodId: "",
      userId: "",
      isReimbursement: false,
    };

    mockTransactionService.createTransaction.and.returnValue(
      of(newTransaction),
    );

    component.handleFormSubmit(newTransaction);

    expect(mockTransactionService.createTransaction).toHaveBeenCalledWith(
      newTransaction,
    );
    expect(component.incomeTransactions).toContain(newTransaction);
  });

  it("should log error if transaction creation fails", () => {
    const transaction: Transaction = {
      name: "Taxi",
      date: "2024-05-03",
      description: "",
      amount: 100,
      notes: "",
      type: TransactionType.Income,
      isCreditCardPurchase: false,
      categoryId: "",
      paymentMethodId: "",
      userId: "",
      isReimbursement: false,
    };

    spyOn(console, "error");
    mockTransactionService.createTransaction.and.returnValue(
      throwError(() => new Error("Error")),
    );

    component.handleFormSubmit(transaction);

    expect(console.error).toHaveBeenCalledWith(
      "Error creating the transaction:",
      jasmine.any(Error),
    );
  });

  it("should not add transaction if creation fails", fakeAsync(() => {
    const transaction = mockTransactions[0];
    mockTransactionService.createTransaction.and.returnValue(throwError(() => new Error("Error")));

    component.incomeTransactions = [];
    component.handleFormSubmit(transaction);
    tick();

    expect(component.incomeTransactions.length).toBe(0);
  }));

  it("should have correct displayedColumns", () => {
    expect(component.displayedColumns).toEqual(["name", "date", "amount"]);
  });
});
