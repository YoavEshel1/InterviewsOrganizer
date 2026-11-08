export interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;
  turnstileToken: string;
}
