using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace microUsuarios.API.Utils
{
    public static class JWTHelper
    {
        public static string GenerarToken(int idCliente, string correo, string rol)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Variables.Token.Llave);

            var claims = new[]
            {
            new Claim("idUser", idCliente.ToString()),
            new Claim(ClaimTypes.Email, correo),
            new Claim(ClaimTypes.Role, rol),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Variables.Token.Expiration),
                Issuer = Variables.Token.Bearer,
                Audience = Variables.Token.Bearer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static ClaimsPrincipal? ValidarToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Variables.Token.Llave);

            try
            {
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = Variables.Token.Bearer,
                    ValidAudience = Variables.Token.Bearer,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // no tolerancia al retraso
                };

                var principal = tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);

                // Opcional: verificar que el algoritmo sea HmacSha256
                if (validatedToken is JwtSecurityToken jwt &&
                    jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }

                return null;
            }
            catch
            {
                return null; // Token inválido
            }
        }

        public static string? GetClaimValue(ClaimsPrincipal principal, string claimType)
        {
            return principal?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }
}
