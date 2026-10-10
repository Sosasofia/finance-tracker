import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { UserSettingsService } from '../../core/services/user-settings.service';
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
  ],
})
export class SettingsComponent implements OnInit {
  private readonly userService = inject(UserSettingsService);
  private dialog = inject(MatDialog);

  profile: UserProfile | null = null;
  preferences: UserPreferences | null = null;

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
  });

  ngOnInit() {
    this.userService.getProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        if (profile.name) {
          this.profileForm.patchValue({ name: profile.name });
        }
      },
      error: (err) => console.error('Error fetching profile:', err),
    });

    this.userService.getPreferences().subscribe({
      next: (preferences) => {
        this.preferences = preferences;
        if (preferences.defaultCurrency) {
          this.preferencesForm.patchValue({ currency: preferences.defaultCurrency });
        }
      },
      error: (err) => console.error('Error fetching preferences:', err),
    });
  }

  onProfileSubmit() {
    if (this.profileForm.invalid) return;

    this.userService.updateProfile({ name: this.profileForm.get('name')?.value || '' }).subscribe({
      next: (updatedProfile) => (this.profile = updatedProfile),
      error: (err) => console.error(err),
    });
  }

  onAvatarChange(e: Event) {
    e.preventDefault();
  }

  onPasswordSubmit() {
    if (this.passwordForm.invalid) return;
    const { currentPassword, newPassword } = this.passwordForm.value;
    if (currentPassword && newPassword) {
      this.userService.changePassword({ currentPassword, newPassword }).subscribe({
        next: () => this.passwordForm.reset(),
        error: (err) => console.error(err),
      });
    }
  }

  onPreferencesSubmit() {
    if (this.preferencesForm.invalid || !this.preferences) return;

    const updatedPreferences: UserPreferences = {
      ...this.preferences,
      defaultCurrency:
        this.preferencesForm.get('currency')?.value || this.preferences.defaultCurrency,
    };

    this.userService.updatePreferences(updatedPreferences).subscribe({
      next: (prefs) => (this.preferences = prefs),
      error: (err) => console.error(err),
    });
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
            console.log('Account deleted successfully');
          },
          error: (err) => console.error(err),
        });
      }
    });
  }
}
