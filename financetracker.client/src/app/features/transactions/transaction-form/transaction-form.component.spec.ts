import { ComponentFixture, TestBed } from "@angular/core/testing";

import { TransactionFormComponent } from "./transaction-form.component";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { TransactionService } from "../../../core/services/transaction.service";
import { of } from "rxjs";
import { Validators } from "@angular/forms";

describe("TransactionFormComponent", () => {
  let component: TransactionFormComponent;
  let fixture: ComponentFixture<TransactionFormComponent>;
  let mockTransactionService: jasmine.SpyObj<TransactionService>;

  beforeEach(async () => {
    mockTransactionService = jasmine.createSpyObj("TransactionService", [
      "getPaymentMethods",
      "getCategories",
    ]);

    mockTransactionService.getPaymentMethods.and.returnValue(of([]));
    mockTransactionService.getCategories.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [TransactionFormComponent, HttpClientTestingModule],
      providers: [
        { provide: TransactionService, useValue: mockTransactionService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TransactionFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  it("load catalog on init", () => {
    const fixture = TestBed.createComponent(TransactionFormComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector("mat-label")?.textContent).toContain(
      "Choose a date",
    );
  });

  it("should emit submitted event with valid transaction data", () => {
    component.transactionType = "expense";
    component.ngOnInit();

    spyOn(component.submitted, "emit");

    component.transactionForm.setValue({
      amount: 100,
      name: "Compra",
      type: "expense",
      description: "Descripción",
      date: new Date(),
      notes: "",
      receiptUrl: "",
      categoryId: null,
      paymentMethodId: null,
      isCreditCardPurchase: false,
      isReimbursement: false,
      installment: {
        number: null,
        interest: null,
      },
      reimbursement: {
        amount: null,
        date: new Date(),
        reason: null,
      },
    });

    component.onSubmit();

    expect(component.submitted.emit).toHaveBeenCalledWith(
      jasmine.objectContaining({
        amount: 100,
        name: "Compra",
        type: "expense",
      }),
    );
  });

  it("should load catalog on init", () => {
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;

    expect(compiled.querySelector("mat-label")?.textContent).toContain("Choose a date");
  });

  it("should emit submitted event with valid transaction data", () => {
    component.transactionType = "expense";
    component.ngOnInit(); 

    spyOn(component.submitted, "emit");

    component.transactionForm.get("installment")?.disable();
    component.transactionForm.get("reimbursement")?.disable();

    component.transactionForm.patchValue({
      amount: 100,
      name: "Test expense",
      type: "expense",
      description: "",
      date: new Date(),
      notes: "",
      receiptUrl: "",
      categoryId: 1,
      paymentMethodId: 1,
      isCreditCardPurchase: false,
      isReimbursement: false,
    });

    component.onSubmit();

    expect(component.submitted.emit).toHaveBeenCalledWith(
      jasmine.objectContaining({
        amount: 100,
        name: "Test expense",
        type: "expense",
      }),
    );
  });

  it("should log a message if the form is invalid", () => {
    component.transactionForm.patchValue({
      amount: null,
      name: null,
      type: null,
      description: null,
      date: new Date(),
      notes: "",
      receiptUrl: "",
      categoryId: 1,
      paymentMethodId: 1,
      isCreditCardPurchase: false,
      isReimbursement: false,
    });

    spyOn(console, "log");
    component.onSubmit();

    expect(console.log).toHaveBeenCalledWith("Formulario inválido");
  });

  it("should enable installment fields when isCreditCardPurchase is true", () => {
    component.transactionType = "expense";
    component.ngOnInit();

    const form = component.transactionForm;
    form.get("isCreditCardPurchase")?.setValue(true);

    const numberControl = form.get("installment.number");
    const interestControl = form.get("installment.interest");

    expect(numberControl?.enabled).toBeTrue();
    expect(interestControl?.enabled).toBeTrue();
    expect(numberControl?.hasValidator(Validators.required)).toBeTrue();
  });

  it("should enable reimbursement fields when isReimbursement is true", () => {
    component.transactionType = "expense";
    component.ngOnInit();

    const form = component.transactionForm;
    form.get("isReimbursement")?.setValue(true);

    const amountControl = form.get("reimbursement.amount");
    const dateControl = form.get("reimbursement.date");
    const reasonControl = form.get("reimbursement.reason");

    expect(amountControl?.enabled).toBeTrue();
    expect(dateControl?.enabled).toBeTrue();
    expect(reasonControl?.enabled).toBeTrue();
  });
});
