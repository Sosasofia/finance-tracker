import { BreakpointObserver } from '@angular/cdk/layout';
import { ComponentFixture, TestBed, fakeAsync, flushMicrotasks } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MatDialog } from '@angular/material/dialog';
import { BehaviorSubject, of } from 'rxjs';

import { TransactionService } from '../../../../core/services/transaction.service';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { Transaction, TransactionType } from '../../../../shared/models/transaction.model';
import { TransactionStore } from '../../state/transaction.store';
import { EditTransactionDialogComponent } from '../edit-transaction-dialog/edit-transaction-dialog.component';
import { TransactionLayoutComponent } from './transaction-layout.component';

describe('TransactionLayoutComponent', () => {
  let component: TransactionLayoutComponent;
  let fixture: ComponentFixture<TransactionLayoutComponent>;
  let dialogOpenSpy: jasmine.Spy;
  let breakpointState$: BehaviorSubject<{ matches: boolean }>;
  let mockStore: {
    transactions: jasmine.Spy;
    isLoading: jasmine.Spy;
    loadTransactions: jasmine.Spy;
    deleteTransaction: jasmine.Spy;
    addTransactionLocal: jasmine.Spy;
  };

  const sampleTransaction: Transaction = {
    id: 'tx-1',
    amount: 123.45,
    name: 'Sample transaction',
    description: 'Test item',
    date: '2026-05-05',
    notes: 'note',
    receiptUrl: '',
    type: TransactionType.Expense,
    isCreditCardPurchase: false,
    categoryId: 'cat-1',
    paymentMethodId: 'pm-1',
    userId: 'user-1',
    isReimbursement: false,
  };

  beforeEach(async () => {
    breakpointState$ = new BehaviorSubject<{ matches: boolean }>({ matches: false });

    mockStore = {
      transactions: jasmine.createSpy('transactions').and.returnValue([sampleTransaction]),
      isLoading: jasmine.createSpy('isLoading').and.returnValue(false),
      loadTransactions: jasmine.createSpy('loadTransactions').and.resolveTo(),
      deleteTransaction: jasmine.createSpy('deleteTransaction').and.resolveTo(),
      addTransactionLocal: jasmine.createSpy('addTransactionLocal'),
    };

    const mockDialog = {
      open: jasmine.createSpy('open'),
    };

    dialogOpenSpy = mockDialog.open as jasmine.Spy;

    const mockTransactionService = jasmine.createSpyObj<TransactionService>('TransactionService', [
      'getPaymentMethods',
      'getCategories',
      'createTransaction',
      'updateTransaction',
      'deleteTransaction',
    ]);

    mockTransactionService.getPaymentMethods.and.returnValue(of([]));
    mockTransactionService.getCategories.and.returnValue(of([]));
    mockTransactionService.createTransaction.and.returnValue(of(sampleTransaction));
    mockTransactionService.updateTransaction.and.returnValue(of(sampleTransaction));
    mockTransactionService.deleteTransaction.and.returnValue(of(undefined));

    await TestBed.configureTestingModule({
      imports: [TransactionLayoutComponent, NoopAnimationsModule],
      providers: [
        { provide: TransactionStore, useValue: mockStore },
        { provide: TransactionService, useValue: mockTransactionService },
        {
          provide: BreakpointObserver,
          useValue: { observe: () => breakpointState$.asObservable() },
        },
        { provide: MatDialog, useValue: mockDialog },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TransactionLayoutComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('transactionType', TransactionType.Expense);
    fixture.detectChanges();
  });

  it('should set the selected transaction on desktop edit', () => {
    component.editTransaction(sampleTransaction);

    expect(component.selectedTransaction()).toEqual(sampleTransaction);
    expect(dialogOpenSpy).not.toHaveBeenCalled();
  });

  it('should open the edit dialog on mobile edit', () => {
    breakpointState$.next({ matches: true });

    const afterClosed$ = of({ success: true, message: 'Updated' });
    dialogOpenSpy.and.returnValue({ afterClosed: () => afterClosed$ });

    component.editTransaction(sampleTransaction);

    expect(dialogOpenSpy).toHaveBeenCalledWith(
      EditTransactionDialogComponent,
      jasmine.objectContaining({
        data: jasmine.objectContaining({
          transaction: sampleTransaction,
          transactionType: TransactionType.Expense,
        }),
      })
    );
  });

  it('should confirm and delete a transaction', fakeAsync(() => {
    dialogOpenSpy.and.returnValue({ afterClosed: () => of(true) });
    component.selectedTransaction.set(sampleTransaction);

    component.deleteTransaction(sampleTransaction.id!);
    flushMicrotasks();

    expect(dialogOpenSpy).toHaveBeenCalledWith(
      ConfirmDialogComponent,
      jasmine.objectContaining({
        data: jasmine.objectContaining({
          confirmButtonText: 'Delete',
          cancelButtonText: 'Cancel',
        }),
      })
    );
    expect(mockStore.deleteTransaction).toHaveBeenCalledWith(sampleTransaction.id!);
    expect(component.selectedTransaction()).toBeNull();
  }));
});
