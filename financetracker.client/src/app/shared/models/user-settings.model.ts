export interface UserProfile {
  id: string;
  name: string;
  email: string;
  profilePicture?: string;
}

export interface UpdateProfileRequest {
  name: string;
  profilePicture?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface UserPreferences {
  theme: 'light' | 'dark';
  notifications: boolean;
}
