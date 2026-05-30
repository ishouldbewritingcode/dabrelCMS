# Future Upgrades
---------------

## TOTP Two-Factor Authentication [DONE - 2026-05-29]
  TOTP is optional per user. Users without a TotpSecret log in exactly as before.
  Files changed:
    - models/CMSUser.cs          — added TotpSecret (string, nullable)
    - Migrations/20260530...     — AddTotpSecret migration (applied)
    - code/JwtUtils.cs           — GeneratePendingTotpToken / ValidatePendingTotpToken
    - code/TotpReplayCache.cs    — new; in-memory replay protection (ConcurrentDictionary)
    - code/Site.cs               — two-step auth: password then TOTP if secret is set
    - code/Common.cs             — GetTotpPage helper
    - wwwroot/totp.htm           — new TOTP code entry page
    - code/AdminUser.cs          — GenerateTotpSetup / ConfirmTotpSetup / DisableTotp
    - wwwroot/designs/admin/dialoguser.htm — TOTP status + enable/disable button
    - code/Admin.cs              — handlers: generatetotp, confirmtotp, disabletotp
  Login flow: POST /auth (password) → if TotpSecret set, issues 5-min pending JWT cookie
              and shows totp.htm → POST /auth (step=totp) → verifies code + replay check → session token.
  Admin setup: User profile dialog shows 2FA status. "Set up 2FA" generates secret + otpauth:// URI.
               User confirms with a code. "Disable 2FA" clears the secret.

## Use Serilog
  * log all authentications
  * log all errors
  * log file uploads
  * log sitemanager tasks
  * should anything else be logged?

## Add some basic caching so we aren't going to the database as often.
  * Admin changes to the page should expire the cache

## Bug - Search doesn't seem to work for items.


