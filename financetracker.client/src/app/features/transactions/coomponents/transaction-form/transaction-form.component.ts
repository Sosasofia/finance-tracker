import {
  Component,
  DestroyRef,
  OnInit,
  effect,
  inject,
  input,
  output,
  signal,
  untracked,
} from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_DATE_LOCALE, MatOptionModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';

import { TransactionService } from '../../../../core/services/transaction.service';
import { Transaction, TransactionType } from '../../../../shared/models/transaction.model';

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatDatepickerModule,
    MatRadioModule,
    MatSelectModule,
    MatOptionModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatIconModule,
  ],
  templateUrl: './transaction-form.component.html',
  styleUrls: ['./transaction-form.component.css'],
  providers: [provideNativeDateAdapter(), { provide: MAT_DATE_LOCALE, useValue: 'en-GB' }],
})
export class TransactionFormComponent implements OnInit {
  public TransactionType = TransactionType;

  transactionType = input<TransactionType>(TransactionType.Expense);
  isLoading = input(false);
  transaction = input<Transaction | null>(null);
  submitted = output<Transaction>();

  cancel = output<void>();
  deleted = output<string>();
  isEditMode = signal(false);
  transactionForm!: FormGroup;

  private fb = inject(FormBuilder);
  private transactionService = inject(TransactionService);
  private destroyRef = inject(DestroyRef);

  paymentMethods = toSignal(this.transactionService.getPaymentMethods(), { initialValue: [] });
  categories = toSignal(this.transactionService.getCategories(), { initialValue: [] });

  constructor() {
    effect(() => {
      const tx = this.transaction();
      if (this.transactionForm) {
        untracked(() => {
          if (tx) {
            this.isEditMode.set(true);
            this.setFormValues(tx);
          } else {
            this.isEditMode.set(false);
            this.resetForm();
          }
        });
      }
    });
  }

  ngOnInit(): void {
    this.prepareForm();

    const tx = this.transaction();
    if (tx) {
      this.isEditMode.set(true);
      this.setFormValues(tx);
    }
  }

  onSubmit() {
    if (this.transactionForm.invalid) {
      this.transactionForm.markAllAsTouched();
      return;
    }

    const formValue = this.transactionForm.value;

    if (!formValue.isCreditCardPurchase) delete formValue.installment;
    if (!formValue.isReimbursement) delete formValue.reimbursement;

    this.submitted.emit(formValue as Transaction);
    this.resetForm();
    this.isEditMode.set(false);
  }

  onDelete() {
    const tx = this.transaction();
    if (tx && tx.id) {
      this.deleted.emit(tx.id);
    }
  }

  get isExpense(): boolean {
    return String(this.transactionType()).toLowerCase() === 'expense';
  }

  get installmentGroup(): FormGroup {
    return this.transactionForm.get('installment') as FormGroup;
  }

  get reimbursementGroup(): FormGroup {
    return this.transactionForm.get('reimbursement') as FormGroup;
  }

  resetForm() {
    this.transactionForm.reset(
      {
        amount: '',
        name: '',
        description: '',
        date: new Date(),
        notes: '',
        receiptUrl: '',
        type: this.transactionType(),
        categoryId: null,
        paymentMethodId: null,
        isCreditCardPurchase: false,
        isReimbursement: false,
      },
      { emitEvent: false }
    );

    if (this.transactionType() === TransactionType.Expense) {
      const installment = this.transactionForm.get('installment') as FormGroup | null;
      const reimbursement = this.transactionForm.get('reimbursement') as FormGroup | null;

      if (installment) {
        installment.reset({ number: 1, interest: 0 }, { emitEvent: false });
        installment.disable({ emitEvent: false });
      }

      if (reimbursement) {
        reimbursement.reset({ amount: null, date: new Date(), reason: '' }, { emitEvent: false });
        reimbursement.disable({ emitEvent: false });
      }
    }

    Object.values(this.transactionForm.controls).forEach((control) => {
      control?.setErrors(null);
      control?.markAsPristine();
      control?.markAsUntouched();
    });
  }

  private prepareForm() {
    this.transactionForm = this.fb.group({
      amount: [null, { validators: [Validators.required, Validators.min(0.01)] }],
      name: ['', { validators: [Validators.required, Validators.minLength(3)] }],
      type: [this.transactionType()],
      description: [''],
      date: [new Date(), { validators: [Validators.required] }],
      notes: [''],
      receiptUrl: [''],
      isCreditCardPurchase: [false],
      isReimbursement: [false],
      categoryId: [null, { validators: [Validators.required] }],
      paymentMethodId: [null, { validators: [Validators.required] }],
    });

    if (String(this.transactionType()).toLowerCase() === 'expense') {
      this.addExpenseControls();
    }
  }

  private addExpenseControls() {
    this.transactionForm.addControl('isCreditCardPurchase', this.fb.control(false));
    this.transactionForm.addControl('isReimbursement', this.fb.control(false));

    this.transactionForm.addControl(
      'installment',
      this.fb.group({
        number: [
          { value: 1, disabled: true },
          [Validators.required, Validators.min(1), Validators.max(12)],
        ],
        interest: [{ value: 0, disabled: true }, [Validators.required, Validators.min(0)]],
      })
    );

    this.transactionForm.addControl(
      'reimbursement',
      this.fb.group({
        amount: [{ value: null, disabled: true }, [Validators.required, Validators.min(0.01)]],
        date: [{ value: new Date(), disabled: true }, Validators.required],
        reason: [{ value: '', disabled: true }, Validators.required],
      })
    );

    this.setupConditionalGroupToggle('isCreditCardPurchase', 'installment');
    this.setupConditionalGroupToggle('isReimbursement', 'reimbursement');
  }

  private setupConditionalGroupToggle(checkboxName: string, groupName: string) {
    const checkbox = this.transactionForm.get(checkboxName);
    const group = this.transactionForm.get(groupName);

    if (checkbox && group) {
      checkbox.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((isEnabled) => {
        this.toggleGroupState(group, isEnabled);
      });
    }
  }

  private toggleGroupState(control: AbstractControl, isEnabled: boolean) {
    if (isEnabled) {
      control.enable();
    } else {
      control.disable();
      control.reset();
    }
  }

  setFormValues(transaction: Transaction): void {
    const formValues = {
      ...transaction,
      date: transaction.date ? new Date(transaction.date) : new Date(),
    };

    this.transactionForm.patchValue(formValues);

    if (transaction.isCreditCardPurchase) {
      this.toggleGroupState(this.installmentGroup, true);
    }
    if (transaction.isReimbursement) {
      this.toggleGroupState(this.reimbursementGroup, true);
    }
  }
}
