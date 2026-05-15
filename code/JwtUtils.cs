using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

//using WebApi.Entities;
//using WebApi.Helpers;

namespace dabrelCMS.code
{
	public interface IJwtUtils
	{
		public string GenerateToken(CMSUser user, string jwtkey, string url);

		public Guid? ValidateToken(string token, string jwtkey, string url);
	}

	public class JwtUtils : IJwtUtils
	{
		public string GenerateToken(CMSUser user, string jwtkey, string url)
		{
			// generate token that is valid for 7 days
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(jwtkey);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] {
					new Claim("id", user.UserId.ToString()),
					new Claim(JwtRegisteredClaimNames.Email, user.Email),
					new Claim(JwtRegisteredClaimNames.Jti,
					Guid.NewGuid().ToString())
				}),
				Expires = DateTime.UtcNow.AddDays(1),
				Issuer = url,
				Audience = url,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		public Guid? ValidateToken(string? token, string jwtkey, string url)
		{
			if (token == null)
				return null;

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(jwtkey);
			try
			{
				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidIssuer = url,
					ValidAudience = url,
					ClockSkew = TimeSpan.Zero
				}, out SecurityToken validatedToken);

				var jwtToken = (JwtSecurityToken)validatedToken;
				var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

				return userId;
			}
			catch
			{
				return null;
			}
		}
	}
}