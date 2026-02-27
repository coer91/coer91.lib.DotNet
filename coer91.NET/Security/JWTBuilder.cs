using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Data;
using System.Text;

namespace coer91.NET
{
    public class JWTBuilder(string _secretKey)
    { 
        protected string _issuer = null;
        protected string _audience = null;
        protected int _expiration = 0;
        protected bool _useUTCDate = true;
        protected IEnumerable<Claim> _claims = [];


        public JWTBuilder SetIssuer(string issuer)
        {
            _issuer = issuer;
            return this;
        }

        public JWTBuilder SetAudience(string audience)
        {
            _audience = audience;
            return this;
        }

        public JWTBuilder SetExpirationMinutes (int expiration)
        { 
            _expiration = (expiration <= 0) ? (60 * 24 * 365 * 10) : expiration;
            return this;
        }

        public JWTBuilder UseUTCDate(bool useUTCDate = true)
        {
            _useUTCDate = useUTCDate;
            return this;
        }

        public JWTBuilder SetClaims(IEnumerable<Claim> claims)
        {
            _claims = claims;
            return this;
        }

        public JWTBuilder AddClaim(string type, string value)
        {
            _claims = _claims.Concat([new Claim(type, value)]);
            return this;
        }

        public JWTBuilder AddClaim(string type, int value)
        {
            _claims = _claims.Concat([new Claim(type, $"{value}")]);
            return this;
        }

        public JWTBuilder AddClaim(string type, float value)
        {
            _claims = _claims.Concat([new Claim(type, $"{value}")]);
            return this;
        }

        public JWTBuilder AddClaim(string type, double value)
        {
            _claims = _claims.Concat([new Claim(type, $"{value}")]);
            return this;
        }

        public JWTBuilder AddClaim(string type, decimal value)
        {
            _claims = _claims.Concat([new Claim(type, $"{value}")]);
            return this;
        }

        public JWTBuilder AddClaim(string type, long value)
        {
            _claims = _claims.Concat([new Claim(type, $"{value}")]);
            return this;
        }

        public JWTBuilder AddClaim(string type, bool value)
        {
            _claims = _claims.Concat([new Claim(type, $"{value}")]);
            return this;
        }

        public JWTBuilder AddClaim(string type, IEnumerable<string> value)
        {
            _claims = _claims.Concat([new Claim(type, $"[{string.Join(',', value.ToArray())}]")]);
            return this;
        }

        public string Build()
        {
            if (string.IsNullOrWhiteSpace(_secretKey)) return string.Empty;

            DateTime expires = _useUTCDate
                ? DateTime.UtcNow.AddMinutes(_expiration)
                : DateTime.Now.AddMinutes(_expiration);

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_secretKey));
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            _claims = _claims
                .Where(x => !x.Type.Equals("ExpirationDate", StringComparison.OrdinalIgnoreCase))
                .Concat([new Claim("ExpirationDate", expires.ToString("yyyy-MM-dd HH:mm:ss"))]);

            JwtSecurityToken securityToken = new(
                issuer: _issuer,
                audience: _audience,
                claims: _claims,
                expires: expires,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
} 