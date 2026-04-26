import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Validators } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of } from 'rxjs';

import { TransactionService } from '../../../core/services/transaction.service';
import { TransactionFormComponent } from './transaction-form.component';

describe('TransactionFormComponent', () => {
  let component: TransactionFormComponent;
  let fixture: ComponentFixture<TransactionFormComponent>;
  let mockTransactionService: jasmine.SpyObj<TransactionService>;

  beforeEach(async () => {
    (window as any).google = {
      accounts: {
        id: {
          initialize: jasmine.createSpy('initialize'),
          renderButton: jasmine.createSpy('renderButton'),
          prompt: jasmine.createSpy('prompt'),
        },
      },
    };

    mockTransactionService = jasmine.createSpyObj('TransactionService', [
      'getPaymentMethods',
      'getCategories',
    ]);

    mockTransactionService.getPaymentMethods.and.returnValue(of([]));
    mockTransactionService.getCategories.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [TransactionFormComponent, NoopAnimationsModule],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: TransactionService, useValue: mockTransactionService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TransactionFormComponent);
    component = fixture.componentInstance;

    fixture.componentRef.setInput('transactionType', 'expense');
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load catalog and render form on init', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('mat-label')?.textContent).toContain('Choose a date');
  });

  it('should emit submitted event with valid expense transaction data', () => {
    const spy = spyOn(component.submitted, 'emit');

    component.transactionForm.patchValue({
      amount: 100,
      name: 'Compra',
      type: 'expense',
      description: 'Descripción',
      date: new Date(),
      notes: '',
      receiptUrl: '',
      categoryId: 'dbfcd470-c014-4d84-8ad1-87321feca829',
      paymentMethodId: '8e4c2145-670e-449f-87f5-c87d23b5e94f',
      isCreditCardPurchase: false,
      isReimbursement: false,
    });

    component.onSubmit();

    expect(spy).toHaveBeenCalledWith(
      jasmine.objectContaining({
        amount: 100,
        name: 'Compra',
        type: 'expense',
      })
    );
  });

  it('should emit submitted event with valid income transaction data', () => {
    fixture.componentRef.setInput('transactionType', 'income');
    fixture.detectChanges();

    spyOn(component.submitted, 'emit');

    component.transactionForm.get('installment')?.disable();
    component.transactionForm.get('reimbursement')?.disable();

    component.transactionForm.patchValue({
      amount: 100,
      name: 'Test income',
      type: 'income',
      categoryId: 'dbfcd470-c014-4d84-8ad1-87321feca829',
      paymentMethodId: '8e4c2145-670e-449f-87f5-c87d23b5e94f',
      isCreditCardPurchase: false,
      isReimbursement: false,
    });

    component.onSubmit();

    expect(component.submitted.emit).toHaveBeenCalledWith(
      jasmine.objectContaining({
        amount: 100,
        name: 'Test income',
        type: 'income',
      })
    );
  });

  it('should log a message if the form is invalid', () => {
    const emitSpy = spyOn(component.submitted, 'emit');
    const markAsTouchedSpy = spyOn(component.transactionForm, 'markAllAsTouched');

    component.transactionForm.get('amount')?.setValue(null); // Invalid value
    component.onSubmit();

    expect(markAsTouchedSpy).toHaveBeenCalled();
    expect(emitSpy).not.toHaveBeenCalled();
  });

  it('should enable installment fields when isCreditCardPurchase is true', () => {
    const form = component.transactionForm;
    form.get('isCreditCardPurchase')?.setValue(true);

    const numberControl = form.get('installment.number');
    const interestControl = form.get('installment.interest');

    expect(numberControl?.enabled).toBeTrue();
    expect(interestControl?.enabled).toBeTrue();
    expect(numberControl?.hasValidator(Validators.required)).toBeTrue();
  });

  it('should enable reimbursement fields when isReimbursement is true', () => {
    const form = component.transactionForm;
    form.get('isReimbursement')?.setValue(true);

    const amountControl = form.get('reimbursement.amount');
    const dateControl = form.get('reimbursement.date');
    const reasonControl = form.get('reimbursement.reason');

    expect(amountControl?.enabled).toBeTrue();
    expect(dateControl?.enabled).toBeTrue();
    expect(reasonControl?.enabled).toBeTrue();
  });
});
