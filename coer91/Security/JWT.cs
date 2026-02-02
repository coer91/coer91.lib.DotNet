using System.Text; 
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt; 
using Microsoft.IdentityModel.Tokens; 
using Microsoft.AspNetCore.Http; 

namespace coer91
{
    public class JWT
    {  
        /// <summary>
        /// Generate a JWT
        /// </summary> 
        public static string GenerateJwt(string secretKey, IEnumerable<Claim> claims, int? expirationInMinutes = 0, bool? useUTCDate = true)
        {
            if (expirationInMinutes is null || expirationInMinutes <= 0) expirationInMinutes = 60 * 24 * 365 * 10;

            DateTime expires = useUTCDate is null || useUTCDate is true
                ? DateTime.UtcNow.AddMinutes((int)expirationInMinutes) 
                : DateTime.Now.AddMinutes((int)expirationInMinutes);  

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256); 

            IEnumerable<Claim> _claims = claims
                .Where(x => !x.Type.Equals("ExpirationDate", StringComparison.OrdinalIgnoreCase))
                .Concat([new Claim("ExpirationDate", expires.ToString("yyyy-MM-dd HH:mm:ss"))]);            

            JwtSecurityToken securityToken = new(
                issuer: null,
                audience: null,
                claims: _claims, 
                expires: expires,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }


        #region GetClaims

        public static IEnumerable<Claim> GetClaims(IHttpContextAccessor httpContextAccessor)
            => GetClaims(httpContextAccessor?.HttpContext);


        public static IEnumerable<Claim> GetClaims(HttpContext context)
            => context is not null ? context.User?.Claims : [];

        #endregion


        #region GetClaim

        /// <summary>
        /// Get a claim
        /// </summary>
        /// <returns>string</returns>
        public static string GetClaim(string claim, IHttpContextAccessor httpContextAccessor)
            => GetClaim(claim, httpContextAccessor?.HttpContext);


        /// <summary>
        /// Get a claim
        /// </summary>
        /// <returns>string</returns>
        public static string GetClaim(string claim, HttpContext context)
        {
            try
            {
                return context is not null
                    ? GetClaims(context).Where(_claim => _claim.Type == claim).FirstOrDefault()?.Value ?? string.Empty
                    : string.Empty;
            }

            catch
            {
                return string.Empty;
            }
        }

        #endregion 
    }
}