export interface UserProfile {
  email: string;
  name: string;
  picture?: string;
  role: string;
  provider: string;
  createdAt: Date;
  lastLoginAt: Date;
}
