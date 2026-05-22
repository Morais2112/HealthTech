using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicaApi.Configuration;
using ClinicaApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ClinicaApi.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public (string Token, DateTime ExpiraEm) Gerar(Usuario usuario)
    {
        var expiraEm = DateTime.UtcNow.AddMinutes(_settings.ExpiresMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Perfil),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiraEm,
            signingCredentials: credenciais);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expiraEm);
    }
}
