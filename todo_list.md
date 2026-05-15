# Future Upgrades
---------------

TOTP Two-Factor Authentication
  Add TOTP-based 2FA to the login flow using the Otp.NET NuGet package (no 3rd party service required).
  Steps:
    1. Add NuGet package: Otp.NET
    2. Add TotpSecret (string, nullable) column to CMSUser model + migration
    3. On 2FA setup: generate secret with KeyGeneration.GenerateRandomKey(20), store Base32-encoded
       value in CMSUser.TotpSecret, display QR code URI to user for pairing with any authenticator app
    4. On login: if TotpSecret is set, prompt for 6-digit code and verify with:
           var totp = new Totp(Base32Encoding.ToBytes(user.TotpSecret));
           bool valid = totp.VerifyTotp(submittedCode, out _, new VerificationWindow(2, 2));
    5. Track used codes per 30s window to prevent replay attacks
