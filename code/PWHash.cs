namespace dabrelCMS.code
{
	using System;
	using System.Linq;
	using System.Security.Cryptography;

	public static class PWHash
	{
		public static string ComputePasswordHash(string password, int salt)
		{
			byte[] saltBytes = BitConverter.GetBytes(salt);
			byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
			byte[] combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];
			Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
			Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

			using (var sha256 = SHA256.Create())
			{
				byte[] hashBytes = sha256.ComputeHash(combinedBytes);
				return Convert.ToBase64String(hashBytes);
			}
		}

		public static bool IsPasswordValid(string passwordToValidate, int salt, string correctPasswordHash)
		{
			byte[] correctPasswordHashBytes = Convert.FromBase64String(correctPasswordHash);
			string hashedPassword = ComputePasswordHash(passwordToValidate, salt);
			return hashedPassword == correctPasswordHash;
		}
	}
}