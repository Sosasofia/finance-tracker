import { CommonModule } from "@angular/common";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { MatTableModule } from "@angular/material/table";
import { of, throwError } from "rxjs";
import { TransactionService } from "../../core/services/transaction.service";
import { Transaction, TransactionType } from "../../models/transaction.model";
import { DashboardComponent } from "./dashboard.component";


describe("DashboardComponent", () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
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
    ]);

    await TestBed.configureTestingModule({
      imports: [DashboardComponent, CommonModule, MatTableModule],
      providers: [
        { provide: TransactionService, useValue: mockTransactionService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  it("should load transactions on init", () => {
    mockTransactionService.getTransactions.and.returnValue(
      of(mockTransactions),
    );

    component.ngOnInit();

    expect(mockTransactionService.getTransactions).toHaveBeenCalled();
    expect(component.transactions).toEqual(mockTransactions);
  });

  it("should handle errors when loading transactions", () => {
    const consoleSpy = spyOn(console, "error");
    mockTransactionService.getTransactions.and.returnValue(
      throwError(() => new Error("Service error")),
    );

    component.loadTransactions();

    expect(consoleSpy).toHaveBeenCalledWith(
      "Error fetching transactions",
      jasmine.any(Error),
    );
  });

  it("should display the correct number of transaction rows", () => {
    mockTransactionService.getTransactions.and.returnValue(of(mockTransactions));

    component.ngOnInit();
    fixture.detectChanges();

    const rows = fixture.nativeElement.querySelectorAll('table tr[mat-row]');
    expect(rows.length).toBe(3);
  });
});
