import { ComponentFixture, fakeAsync, TestBed, tick } from "@angular/core/testing";

import { of, throwError } from "rxjs";
import { TransactionService } from "../../core/services/transaction.service";
import { Transaction, TransactionType } from "../../models/transaction.model";
import { ExpenseComponent } from "./expense.component";

describe("ExpenseComponent", () => {
  let component: ExpenseComponent;
  let fixture: ComponentFixture<ExpenseComponent>;
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
      "createTransaction"
    ]);

    await TestBed.configureTestingModule({
      declarations: [],
      imports: [ExpenseComponent],
      providers: [
        { provide: TransactionService, useValue: mockTransactionService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ExpenseComponent);
    component = fixture.componentInstance;
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });


  it("should load only expense transactions on init", fakeAsync(() => {
    mockTransactionService.getTransactions.and.returnValue(of(mockTransactions));

    component.loadExpenseTransactions();
    tick();

    expect(mockTransactionService.getTransactions).toHaveBeenCalled();
    expect(component.expenseTransactions.every(t => t.type === TransactionType.Expense)).toBeTrue();
    expect(component.expenseTransactions.length).toBe(1);
  }));

  it("should handle form submit and add new transaction to list", fakeAsync(() => {
    const newTransaction: Transaction = {
      name: "Taxi",
      date: "2024-05-03",
      description: "Airport",
      amount: 150,
      notes: "",
      type: TransactionType.Expense,
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
    tick();

    expect(mockTransactionService.createTransaction).toHaveBeenCalledWith(
      newTransaction,
    );
    expect(component.expenseTransactions).toContain(newTransaction);
  }));

  it("should log error if transaction creation fails", () => {
    const transaction: Transaction = {
      name: "Taxi",
      date: "2024-05-03",
      description: "",
      amount: 100,
      notes: "",
      type: TransactionType.Expense,
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

    component.expenseTransactions = [];
    component.handleFormSubmit(transaction);
    tick();

    expect(component.expenseTransactions.length).toBe(0);
  }));

  it("should have correct displayedColumns", () => {
    expect(component.displayedColumns).toEqual(["name", "date", "amount"]);
  });
});
