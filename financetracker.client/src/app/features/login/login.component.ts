import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import {
    FormBuilder,
    FormGroup,
    ReactiveFormsModule,
    Validators,
} from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { Router } from "@angular/router";

import { AuthService } from "../../core/auth/auth.service";

@Component({
  selector: "app-login",
  standalone: true,
  templateUrl: "./login.component.html",
  styleUrl: "./login.component.css",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
  ],
})
export class LoginComponent {
  form: FormGroup;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
  ) {
    this.form = this.fb.group({
      username: ["", Validators.required],
      password: ["", Validators.required],
    });
  }

  onSubmit() {
    if (this.form.invalid) {
      console.log("Form is invalid");
      return;
    }

    // Handle login logic here
    this.authService.login(this.form.value).subscribe({
      next: () => {
        this.router.navigate(["/dashboard"]); // Navigate to the dashboard after successful login
      },
      error: (err) => {
        console.error("Login failed:", err);
        this.errorMessage = err.error || "Login failed. Please try again.";
      },
    });
  }
}
