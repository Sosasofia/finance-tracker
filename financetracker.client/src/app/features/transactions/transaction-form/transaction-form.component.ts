import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatCheckboxModule } from "@angular/material/checkbox";
import {
  MAT_DATE_LOCALE,
  MatOptionModule,
  provideNativeDateAdapter,
} from "@angular/material/core";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatRadioModule } from "@angular/material/radio";
import { MatSelectModule } from "@angular/material/select";

import { TransactionService } from "../../../core/services/transaction.service";
import {
  Transaction,
  TransactionType,
} from "../../../models/transaction.model";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatIconModule } from "@angular/material/icon";
import { Subscription } from "rxjs";

@Component({
  selector: "app-transaction-form",
  standalone: true,
  imports: [
    CommonModule,
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
  templateUrl: "./transaction-form.component.html",
  styleUrls: ["./transaction-form.component.css"],
  providers: [
    provideNativeDateAdapter(),
    { provide: MAT_DATE_LOCALE, useValue: "en-GB" },
  ],
})
export class TransactionFormComponent implements OnInit {
  public TransactionType = TransactionType;
  @Output() submitted = new EventEmitter<Transaction>();
  @Input() transactionType: "income" | "expense" = "expense";
  @Input() isLoading: boolean = false;

  @Input() set transaction(data: Transaction | null) {
    if (data) {
      this.isEditMode = true;
      this.setFormValues(data);
    }
  }
  isEditMode: boolean = false;

  transactionForm!: FormGroup;

  // Catalogs
  paymentMethods: any[] = [];
  categories: any[] = [];

  private subscriptions = new Subscription();

  constructor(
    private fb: FormBuilder,
    private transactionService: TransactionService,
  ) { }

  ngOnInit(): void {
    this.loadCatalog();
    this.prepareForm();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  onSubmit() {
    if (this.transactionForm.invalid) {
      this.transactionForm.markAllAsTouched();
      return;
    }

    const formValue = this.transactionForm.value;

    if (!formValue.isCreditCardPurchase) {
      delete formValue.installment;
    }
    if (!formValue.isReimbursement) {
      delete formValue.reimbursement;
    }

    this.submitted.emit(formValue as Transaction);

    this.isEditMode = false;
  }

  private loadCatalog() {
    this.subscriptions.add(
      this.transactionService.getPaymentMethods().subscribe((data) => {
        this.paymentMethods = data;
      }),
    );
    this.subscriptions.add(
      this.transactionService.getCategories().subscribe((data) => {
        this.categories = data;
      }),
    );
  }

  get isExpense(): boolean {
    return this.transactionForm.get("type")?.value === "expense";
  }

  get installmentGroup(): FormGroup {
    return this.transactionForm.get("installment") as FormGroup;
  }

  get reimbursementGroup(): FormGroup {
    return this.transactionForm.get("reimbursement") as FormGroup;
  }

  resetForm() {
    this.transactionForm.reset(
      {
        amount: "",
        name: "",
        description: "",
        date: new Date(),
        notes: "",
        receiptUrl: "",
        type: this.transactionType,
        categoryId: null,
        paymentMethodId: null,
        isCreditCardPurchase: false,
        isReimbursement: false,
      },
      { emitEvent: false },
    );

    if (this.transactionType === "expense") {
      const installment = this.transactionForm.get("installment") as FormGroup | null;
      const reimbursement = this.transactionForm.get("reimbursement") as FormGroup | null;

      if (installment) {
        installment.reset({ number: 1, interest: 0 }, { emitEvent: false });
        installment.disable({ emitEvent: false });
      }

      if (reimbursement) {
        reimbursement.reset({ amount: null, date: new Date(), reason: "" }, { emitEvent: false });
        reimbursement.disable({ emitEvent: false });
      }
    }

    Object.keys(this.transactionForm.controls).forEach((key) => {
      const control = this.transactionForm.get(key);
      control?.setErrors(null);
      control?.markAsPristine();
      control?.markAsUntouched();
    });
  }

  /// Prepares the transaction form.
  private prepareForm() {
    this.transactionForm = this.fb.group({
      amount: [
        null,
        { validators: [Validators.required, Validators.min(0.01)] },
      ],
      name: [
        "",
        { validators: [Validators.required, Validators.minLength(3)] },
      ],
      type: [this.transactionType],
      description: [""],
      date: [new Date(), { validators: [Validators.required] }],
      notes: [""],
      receiptUrl: [""],
      isCreditCardPurchase: [false],
      isReimbursement: [false],
      categoryId: [null, { validators: [Validators.required] }],
      paymentMethodId: [null, { validators: [Validators.required] }],
    });

    if (this.transactionType === "expense") {
      this.addExpenseControls();
    }
  }

  /// Sets up a listener to toggle a form group based on a checkbox control.
  private addExpenseControls() {
    this.transactionForm.addControl("isCreditCardPurchase", this.fb.control(false));
    this.transactionForm.addControl("isReimbursement", this.fb.control(false));

    this.transactionForm.addControl(
      "installment",
      this.fb.group({
        number: [{ value: 1, disabled: true }, [Validators.required, Validators.min(1), Validators.max(12)]],
        interest: [{ value: 0, disabled: true }, [Validators.required, Validators.min(0)]],
      })
    );
    this.transactionForm.addControl(
      "reimbursement",
      this.fb.group({
        amount: [{ value: null, disabled: true }, [Validators.required, Validators.min(0.01)]],
        date: [{ value: new Date(), disabled: true }, Validators.required],
        reason: [{ value: "", disabled: true }, Validators.required],
      })
    );

    this.setupConditionalGroupToggle(
      "isCreditCardPurchase",
      "installment"
    );
    this.setupConditionalGroupToggle(
      "isReimbursement",
      "reimbursement"
    );
  }

  /// Sets up a listener to toggle a form group based on a checkbox control.
  private setupConditionalGroupToggle(checkboxName: string, groupName: string) {
    const checkbox = this.transactionForm.get(checkboxName);
    const group = this.transactionForm.get(groupName);

    if (checkbox && group) {
      this.subscriptions.add(
        checkbox.valueChanges.subscribe((isEnabled) => {
          this.toggleGroupState(group, isEnabled);
        })
      );
    }
  }

  /// Toggles the enabled state of a form group.
  private toggleGroupState(control: AbstractControl, isEnabled: boolean) {
    if (isEnabled) {
      control.enable();
    } else {
      control.disable();
      control.reset();
    }
  }

  /// Sets the form values for editing an existing transaction.
  setFormValues(transaction: Transaction): void {
    this.isEditMode = true;
    this.transactionForm.patchValue(transaction);

    if (transaction.isCreditCardPurchase) {
      this.toggleGroupState(this.installmentGroup, true);
    }
    if (transaction.isReimbursement) {
      this.toggleGroupState(this.reimbursementGroup, true);
    }
  }
}
