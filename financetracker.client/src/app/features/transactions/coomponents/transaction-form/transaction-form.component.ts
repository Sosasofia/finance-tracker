import {
  computed,
  Component,
  effect,
  inject,
  input,
  output,
  signal,
  untracked,
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
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
import { MatTooltipModule } from '@angular/material/tooltip';

import { TransactionService } from '../../../../core/services/transaction.service';
import { Transaction, TransactionType } from '../../../../shared/models/transaction.model';
import { ExtractedReceiptData } from '../../../receipt-uploader/receipt-data.model';

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
    MatTooltipModule,
  ],
  templateUrl: './transaction-form.component.html',
  styleUrls: ['./transaction-form.component.css'],
  providers: [provideNativeDateAdapter(), { provide: MAT_DATE_LOCALE, useValue: 'en-GB' }],
})
export class TransactionFormComponent {
  public TransactionType = TransactionType;

  transactionType = input<TransactionType>(TransactionType.Expense);
  isLoading = input(false);
  transaction = input<Transaction | null>(null);
  scannedData = input<ExtractedReceiptData | null>(null);

  submitted = output<Transaction>();
  formCancel = output<void>();
  deleted = output<string>();
  openScanner = output<void>();

  isEditMode = signal(false);
  merchantConfidence = signal<number | null>(null);
  amountConfidence = signal<number | null>(null);
  dateConfidence = signal<number | null>(null);
  categoryConfidence = signal<number | null>(null);
  paymentMethodConfidence = signal<number | null>(null);

  suggestedCategoryText = signal<string | null>(null);
  suggestedPaymentText = signal<string | null>(null);

  needsReview = computed(() => {
    const dConf = this.dateConfidence();
    const aConf = this.amountConfidence();
    const mConf = this.merchantConfidence();

    return (
      (dConf !== null && dConf < 0.85) ||
      (aConf !== null && aConf < 0.85) ||
      (mConf !== null && mConf < 0.85)
    );
  });

  private fb = inject(FormBuilder);
  private transactionService = inject(TransactionService);

  paymentMethods = toSignal(this.transactionService.getPaymentMethods(), { initialValue: [] });
  categories = toSignal(this.transactionService.getCategories(), { initialValue: [] });

  transactionForm: FormGroup = this.buildForm();

  constructor() {
    effect(() => {
      const tx = this.transaction();
      untracked(() => {
        if (tx) {
          this.isEditMode.set(true);
          this.setFormValues(tx);
        } else {
          this.isEditMode.set(false);
          this.resetForm();
        }
      });
    });

    effect(() => {
      const data = this.scannedData();
      if (!data) return;

      untracked(() => {
        this.merchantConfidence.set(data?.merchantNameConfidence || null);
        this.amountConfidence.set(data?.totalAmountConfidence || null);
        this.dateConfidence.set(data?.transactionDateConfidence || null);

        this.categoryConfidence.set(data?.categoryConfidence || null);
        this.paymentMethodConfidence.set(data?.paymentMethodConfidence || null);

        this.suggestedCategoryText.set(data.rawCategoryText);
        this.suggestedPaymentText.set(data.rawPaymentMethodText);

        const itemsText = data?.lineItems?.length ? `Items: ${data.lineItems.join(', ')}` : '';

        this.transactionForm.patchValue({
          name: data?.merchantName || '',
          amount: data?.amount || null,
          date: data?.transactionDate ? new Date(data.transactionDate + 'T00:00:00') : new Date(),
          description: itemsText,
          categoryId: data?.categoryId || null,
          paymentMethodId: data?.paymentMethodId || null,
        });
      });
    });
  }

  private buildForm(): FormGroup {
    const form = this.fb.group({
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
      this.addExpenseControls(form);
    }

    return form;
  }

  private addExpenseControls(form: FormGroup) {
    form.addControl(
      'installment',
      this.fb.group({
        number: [
          { value: 1, disabled: true },
          [Validators.required, Validators.min(1), Validators.max(12)],
        ],
        interest: [{ value: 0, disabled: true }, [Validators.required, Validators.min(0)]],
      })
    );

    form.addControl(
      'reimbursement',
      this.fb.group({
        amount: [{ value: null, disabled: true }, [Validators.required, Validators.min(0.01)]],
        date: [{ value: new Date(), disabled: true }, Validators.required],
        reason: [{ value: '', disabled: true }, Validators.required],
      })
    );

    this.setupConditionalGroupToggle(form, 'isCreditCardPurchase', 'installment');
    this.setupConditionalGroupToggle(form, 'isReimbursement', 'reimbursement');
  }

  private setupConditionalGroupToggle(form: FormGroup, checkboxName: string, groupName: string) {
    const checkbox = form.get(checkboxName);
    const group = form.get(groupName);

    if (checkbox && group) {
      const isEnabledSignal = toSignal(checkbox.valueChanges, { initialValue: checkbox.value });
      effect(() => {
        const isEnabled = isEnabledSignal();
        untracked(() => {
          this.toggleGroupState(group, isEnabled);
        });
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
    if (tx?.id) {
      this.deleted.emit(tx.id);
    }
  }

  onCancel() {
    this.resetForm();
    this.formCancel.emit();
  }

  isMobile() {
    return window.innerWidth < 600;
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
    const defaultValues = {
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
    };

    this.transactionForm.reset(defaultValues, { emitEvent: false });

    if (this.isExpense) {
      this.installmentGroup?.reset({ number: 1, interest: 0 }, { emitEvent: false });
      this.installmentGroup?.disable({ emitEvent: false });

      this.reimbursementGroup?.reset(
        { amount: null, date: new Date(), reason: '' },
        { emitEvent: false }
      );
      this.reimbursementGroup?.disable({ emitEvent: false });
    }

    this.categoryConfidence.set(null);
    this.paymentMethodConfidence.set(null);
    this.suggestedCategoryText.set(null);
    this.suggestedPaymentText.set(null);

    Object.values(this.transactionForm.controls).forEach((control) => {
      control?.setErrors(null);
      control?.markAsPristine();
      control?.markAsUntouched();
    });
  }
}
