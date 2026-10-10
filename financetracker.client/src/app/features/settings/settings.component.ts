import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { UserSettingsService } from '../../core/services/user-settings.service';
import { UserSettingsStateService } from '../../core/services/user-settings-state.service';
import { UserPreferences } from '../../shared/models/user-preferences.model';
import { UserProfile } from '../../shared/models/user-profile.model';
import {
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-settings',
  standalone: true,
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.css',
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatSnackBarModule,
  ],
})
export class SettingsComponent implements OnInit {
  private readonly userService = inject(UserSettingsService);
  private readonly userSettingsState = inject(UserSettingsStateService);
  private readonly snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  profile: UserProfile | null = null;
  preferences: UserPreferences | null = null;
  readonly isSavingProfile = signal(false);
  readonly isSavingPreferences = signal(false);
  readonly isSavingPassword = signal(false);

  profileForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  passwordForm = new FormGroup({
    currentPassword: new FormControl('', Validators.required),
    newPassword: new FormControl('', Validators.required),
    confirmPassword: new FormControl('', Validators.required),
  });

  preferencesForm = new FormGroup({
    currency: new FormControl('', Validators.required),
    dateFormat: new FormControl('', Validators.required),
  });

  ngOnInit() {
    this.userSettingsState.load().subscribe({
      next: ([profile, preferences]) => {
        this.profile = profile;
        this.profileForm.patchValue({ name: profile.name });
        this.preferences = preferences;
        this.preferencesForm.patchValue({
          currency: preferences.defaultCurrency,
          dateFormat: preferences.dateFormat,
        });
      },
      error: () => this.showError('Unable to load your settings. Please try again.'),
    });
  }

  onProfileSubmit() {
    if (this.profileForm.invalid || this.isSavingProfile()) return;

    this.isSavingProfile.set(true);
    this.userSettingsState
      .updateProfile({
        name: this.profileForm.get('name')?.value || '',
      })
      .subscribe({
        next: (updatedProfile) => {
          this.profile = updatedProfile;
          this.snackBar.open('Profile updated successfully.', 'Close', { duration: 3000 });
        },
        error: () => this.showError('Unable to update your profile.'),
        complete: () => this.isSavingProfile.set(false),
      });
  }

  onAvatarChange(e: Event) {
    e.preventDefault();
  }

  onPasswordSubmit() {
    if (this.passwordForm.invalid || this.isSavingPassword()) return;
    const { currentPassword, newPassword } = this.passwordForm.value;
    if (currentPassword && newPassword) {
      this.isSavingPassword.set(true);
      this.userService.changePassword({ currentPassword, newPassword }).subscribe({
        next: () => {
          this.passwordForm.reset();
          this.snackBar.open('Password updated successfully.', 'Close', { duration: 3000 });
        },
        error: () => this.showError('Unable to update your password.'),
        complete: () => this.isSavingPassword.set(false),
      });
    }
  }

  onPreferencesSubmit() {
    if (this.preferencesForm.invalid || !this.preferences || this.isSavingPreferences()) return;

    this.isSavingPreferences.set(true);
    const updatedPreferences: UserPreferences = {
      ...this.preferences,
      defaultCurrency:
        this.preferencesForm.get('currency')?.value || this.preferences.defaultCurrency,
      dateFormat: this.preferencesForm.get('dateFormat')?.value || this.preferences.dateFormat,
    };

    this.userSettingsState.updatePreferences(updatedPreferences).subscribe({
      next: (prefs) => {
        this.preferences = prefs;
        this.snackBar.open('Preferences updated successfully.', 'Close', { duration: 3000 });
      },
      error: () => this.showError('Unable to update your preferences.'),
      complete: () => this.isSavingPreferences.set(false),
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Dismiss', { duration: 5000 });
    this.isSavingProfile.set(false);
    this.isSavingPreferences.set(false);
    this.isSavingPassword.set(false);
  }

  onDeleteAccount() {
    const dialogData: ConfirmDialogData = {
      title: 'Confirm Deletion',
      message: 'Are you sure you want to delete the account? This action cannot be undone.',
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel',
    };

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: dialogData,
      panelClass: 'rounded-dialog',
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((isConfirmed: boolean) => {
      if (isConfirmed) {
        this.userService.deleteAccount().subscribe({
          next: () => {
            this.snackBar.open('Account deleted successfully.', 'Close', { duration: 3000 });
            window.location.href = '/login';
          },
          error: () => this.showError('Unable to delete your account.'),
        });
      }
    });
  }
}
