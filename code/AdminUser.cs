using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;

namespace dabrelCMS.code
{
	public static class AdminUser
	{
		public static string GetUserForm(CMSUser authUser)
		{
			string userformPath = $"{Common.WebRootPath}\\designs\\admin\\dialoguser.htm";
			string html = Common.GetFileText(userformPath);
			html = html.Replace("{{userid}}", authUser.UserId.ToString());
			html = html.Replace("{{siteid}}", authUser.SiteId.ToString());
			html = html.Replace("{{email}}", authUser.Email.ToString());
			html = html.Replace("{{firstname}}", authUser.FirstName.ToString());
			html = html.Replace("{{lastname}}", authUser.LastName.ToString());
			html = html.Replace("{{mobile}}", authUser.Mobile.ToString());
			html = html.Replace("{{roles}}", authUser.Roles.ToString());
			bool totpEnabled = !string.IsNullOrEmpty(authUser.TotpSecret);
			html = html.Replace("{{totp_status}}", totpEnabled ? "Enabled" : "Disabled");
			html = html.Replace("{{totp_button}}", totpEnabled
				? "<button type=\"button\" hx-post=\"/admin/disabletotp\" hx-target=\"#totp-section\" hx-swap=\"innerHTML\">Disable 2FA</button>"
				: "<button type=\"button\" hx-post=\"/admin/generatetotp\" hx-target=\"#totp-section\" hx-swap=\"innerHTML\">Set up 2FA</button>");
			return html;
		}

		public static string GenerateTotpSetup(CMSUser authUser)
		{
			byte[] secretBytes = KeyGeneration.GenerateRandomKey(20);
			string base32Secret = Base32Encoding.ToString(secretBytes);
			string issuer = "dabrelCMS";
			string otpauthUri = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(authUser.Email)}?secret={base32Secret}&issuer={Uri.EscapeDataString(issuer)}&algorithm=SHA1&digits=6&period=30";

			var qrGenerator = new QRCodeGenerator();
			var qrData = qrGenerator.CreateQrCode(otpauthUri, QRCodeGenerator.ECCLevel.Q);
			var qrCode = new PngByteQRCode(qrData);
			byte[] qrBytes = qrCode.GetGraphic(6);
			string qrBase64 = Convert.ToBase64String(qrBytes);

			return $@"<div id=""totp-section"">
<p><strong>2FA Status:</strong> Pending setup — scan the QR code with your authenticator app, then enter a code to confirm.</p>
<p><img src=""data:image/png;base64,{qrBase64}"" alt=""TOTP QR Code"" style=""width:200px;height:200px;"" /></p>
<p><strong>Manual entry key:</strong> {base32Secret}</p>
<form hx-post=""/admin/confirmtotp"" hx-target=""#totp-section"" hx-swap=""innerHTML"">
	<input type=""hidden"" name=""totpsecret"" value=""{base32Secret}"" />
	<input type=""text"" name=""totpcode"" inputmode=""numeric"" autocomplete=""one-time-code"" maxlength=""6"" placeholder=""Enter code to confirm"" style=""width:200px;"" />
	<button type=""submit"">Confirm &amp; Enable 2FA</button>
</form>
</div>";
		}

		public static string ConfirmTotpSetup(CMSDbContext dbcontext, HttpContext context, Guid userId)
		{
			string secret = context.Request.Form["totpsecret"].ToString();
			string code = context.Request.Form["totpcode"].ToString().Trim();

			if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(code))
				return "<div id=\"totp-section\"><p style=\"color:red;\">Missing secret or code.</p></div>";

			var totp = new Totp(Base32Encoding.ToBytes(secret));
			long windowUsed;
			bool valid = totp.VerifyTotp(code, out windowUsed, new VerificationWindow(1, 1));
			if (!valid)
				return $"<div id=\"totp-section\"><p style=\"color:red;\">Code was not valid. Please re-scan and try again.</p></div>";

			var user = dbcontext.CMSUsers.Find(userId);
			if (user == null)
				return "<div id=\"totp-section\"><p style=\"color:red;\">User not found.</p></div>";

			user.TotpSecret = secret;
			dbcontext.SaveChanges();
			return "<div id=\"totp-section\"><p><strong>2FA Status:</strong> Enabled <button type=\"button\" hx-post=\"/admin/disabletotp\" hx-target=\"#totp-section\" hx-swap=\"innerHTML\">Disable 2FA</button></p></div>";
		}

		public static string DisableTotp(CMSDbContext dbcontext, Guid userId)
		{
			var user = dbcontext.CMSUsers.Find(userId);
			if (user == null)
				return "<div id=\"totp-section\"><p style=\"color:red;\">User not found.</p></div>";

			user.TotpSecret = null;
			dbcontext.SaveChanges();
			return "<div id=\"totp-section\"><p><strong>2FA Status:</strong> Disabled <button type=\"button\" hx-post=\"/admin/generatetotp\" hx-target=\"#totp-section\" hx-swap=\"innerHTML\">Set up 2FA</button></p></div>";
		}

		public static string SaveUserForm(CMSDbContext dbcontext, HttpContext context, Guid userid)
		{
			CMSUser tempUser = dbcontext.CMSUsers.Where(u => u.UserId == userid).FirstOrDefault();
			tempUser.Email = context.Request.Form["email"].ToString();
			tempUser.FirstName = context.Request.Form["firstname"].ToString();
			tempUser.LastName = context.Request.Form["lastname"].ToString();
			tempUser.Mobile = context.Request.Form["mobile"].ToString();

			if (context.Request.Form["password"].ToString().Length > 0)
			{
				if (context.Request.Form["password"].ToString() == context.Request.Form["vpassword"].ToString())
				{
					int salt = GetRandomSalt();
					tempUser.Salt = salt;
					tempUser.Password = PWHash.ComputePasswordHash(context.Request.Form["password"].ToString(), salt);
				}
			}

			dbcontext.SaveChanges();
			context.Response.Headers["Content-Type"] = "text/html";
			context.Response.StatusCode = StatusCodes.Status200OK;
			return "<div>User saved successfully.</div>";
		}

		private static int GetRandomSalt()
		{
			Random rand = new Random();
			return rand.Next(10, 99999);
		}


	}
}
