import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import {
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
  @Input() initialTransactionData: Transaction | null = null;
  @Input() isEditMode: boolean = false; 
  @Input() isLoading: boolean = false; 
  @Input() apiSuccess: boolean | null = null; 
  @Input() apiMessage: string | null = null; 

  transactionForm!: FormGroup;

  // Catalogs
  paymentMethods: any[] = [];
  categories: any[] = [];

  constructor(
    private fb: FormBuilder,
    private transactionService: TransactionService,
  ) {}

  ngOnInit(): void {
    this.loadCatalog();
    this.prepareForm();

    if (this.initialTransactionData) {
      this.setFormValues(this.initialTransactionData);
    }

    if (this.transactionType === "expense") {
      this.isCreditCardPurchaseListener();
      this.isReimbursementListener();
    }
  }

  onSubmit() {
    if (this.transactionForm.invalid) {
      console.log("Invalid form submission");
      Object.keys(this.transactionForm.controls).forEach((key) => {
        const control = this.transactionForm.get(key);
        if (control && control.invalid) {
          console.log(`❌ Field "${key}" is invalid:`, control.errors);
        }
      });
      this.transactionForm.markAllAsTouched();
      return;
    }

    const transaction: Transaction = {
      ...this.transactionForm.value,
    };

    if (this.transactionForm.get("isCreditCardPurchase")?.value === false) {
      delete transaction.installment;
    }
    if (this.transactionForm.get("isReimbursement")?.value === false) {
      delete transaction.reimbursement;
    }

    this.submitted.emit(transaction);
  }

  loadCatalog() {
    this.transactionService.getPaymentMethods().subscribe((data) => {
      this.paymentMethods = data;
    });
    this.transactionService.getCategories().subscribe((data) => {
      this.categories = data;
    });
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
    console.log("Resetting form");
    this.transactionForm.reset(
      {
        amount: 0.01,
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
        // Reset nested groups explicitly to their initial disabled values
        installment: { number: 1, interest: 0 },
        reimbursement: { amount: 0.01, date: new Date(), reason: "" },
      },
      { emitEvent: false },
    );

    if (this.transactionType === "expense") {
      // Force the listeners to re-evaluate based on the reset checkbox values
      // This will correctly disable the nested groups and clear their validators if checkboxes are false
      this.isCreditCardPurchaseListener();
      this.isReimbursementListener();

      //this.transactionForm.get("categoryId")?.enable({ emitEvent: false });
      //this.transactionForm.get("paymentMethodId")?.enable({ emitEvent: false });
      // this.transactionForm
      //   .get("categoryId")
      //   ?.setValidators([Validators.required]);
      // this.transactionForm
      //   .get("paymentMethodId")
      //   ?.setValidators([Validators.required]);
      // this.transactionForm.get("categoryId")?.updateValueAndValidity();
      // this.transactionForm.get("paymentMethodId")?.updateValueAndValidity();
    }

    Object.keys(this.transactionForm.controls).forEach((key) => {
      const control = this.transactionForm.get(key);
      control?.setErrors(null);
      control?.markAsPristine();
      control?.markAsUntouched();
    });
  }

  setFormValues(transaction: Transaction): void {
    this.transactionForm.patchValue({
      id: transaction.id,
      name: transaction.name,
      amount: transaction.amount,
      date: transaction.date ? new Date(transaction.date) : new Date(),
      description: transaction.description || "",
      notes: transaction.notes || "",
      receiptUrl: transaction.receiptUrl || "",
      type: transaction.type,
      categoryId: transaction.categoryId ? transaction.categoryId : null,
      paymentMethodId: transaction.paymentMethodId
        ? transaction.paymentMethodId
        : null,
      isCreditCardPurchase: transaction.isCreditCardPurchase,
    });

    // Handle installment group only if it exists in the form (i.e., it's an expense)
    const installmentGroup = this.installmentGroup;
    if (transaction.installment && installmentGroup) {
      // Check if data exists AND group exists
      installmentGroup.patchValue(transaction.installment);
      // Manually trigger the listener logic to set validators/enable group if needed
      this.setGroupValidators(
        installmentGroup,
        transaction.isCreditCardPurchase,
        [
          {
            key: "number",
            validators: [
              Validators.required,
              Validators.min(1),
              Validators.max(12),
            ],
          },
          {
            key: "interest",
            validators: [Validators.required, Validators.min(0)],
          },
        ],
      );
    }

    // Handle reimbursement group only if it exists in the form (i.e., it's an expense)
    const reimbursementGroup = this.reimbursementGroup;
    if (transaction.reimbursement && reimbursementGroup) {
      // Check if data exists AND group exists
      reimbursementGroup.patchValue(transaction.reimbursement);
      // Manually trigger the listener logic to set validators/enable group if needed
      this.setGroupValidators(reimbursementGroup, transaction.isReimbursement, [
        {
          key: "amount",
          validators: [Validators.required, Validators.min(0.01)],
        },
        { key: "date", validators: [Validators.required] },
        { key: "reason", validators: [Validators.required] },
      ]);
    }
  }

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
      this.transactionForm.addControl(
        "installment",
        this.fb.group({
          number: [{ value: null, disabled: true }],
          interest: [{ value: 0, disabled: true }],
        }),
      );
      this.transactionForm
        .get("installment.number")
        ?.setValidators([
          Validators.min(1),
          Validators.required,
          Validators.max(12),
        ]);
      this.transactionForm
        .get("installment.interest")
        ?.setValidators([Validators.min(0), Validators.required]);

      this.transactionForm.addControl(
        "reimbursement",
        this.fb.group({
          amount: [{ value: null, disabled: true }],
          date: [{ value: new Date(), disabled: true }],
          reason: [{ value: null, disabled: true }],
        }),
      );
    }
    // this.isCreditCardPurchaseListener();
    // this.isReimbursementListener();
  }

  private isCreditCardPurchaseListener() {
    this.transactionForm
      .get("isCreditCardPurchase")
      ?.valueChanges.subscribe((isChecked) => {
        const installmentGroup = this.installmentGroup;
        this.setGroupValidators(installmentGroup, isChecked, [
          {
            key: "number",
            validators: [Validators.required, Validators.min(1)],
          },
          {
            key: "interest",
            validators: [Validators.required, Validators.min(0)],
          },
        ]);
      });
  }

  private isReimbursementListener() {
    this.transactionForm
      .get("isReimbursement")
      ?.valueChanges.subscribe((isChecked) => {
        const reimburstmentGroup = this.reimbursementGroup;

        this.setGroupValidators(reimburstmentGroup, isChecked, [
          {
            key: "amount",
            validators: [Validators.required, Validators.min(1)],
          },
          { key: "date", validators: [Validators.required] },
          { key: "reason", validators: [Validators.required] },
        ]);
      });
  }

  private setGroupValidators(
    group: FormGroup,
    enable: boolean,
    controls: { key: string; validators: any[] }[],
  ) {
    controls.forEach(({ key, validators }) => {
      const ctrl = group.get(key);
      if (enable) {
        ctrl?.setValidators(validators);
        ctrl?.enable({ emitEvent: false });
      } else {
        ctrl?.clearValidators();
        ctrl?.reset();
        ctrl?.disable({ emitEvent: false });
      }
      ctrl?.updateValueAndValidity();
    });
  }
}
