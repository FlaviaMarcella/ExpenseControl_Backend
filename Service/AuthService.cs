using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseControl.Api.Model.Entity;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseControl.Api.Service;

// 2. Serviço de Autenticação
public class AuthService(IConfiguration configuration)
{
    // Cadastro: Transforma senha em Hash (sal e hash embutidos)
    public string RegisterPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    // Login: Verifica a senha com o Hash
    public string? LoginAndGenerateToken(User user, string password)
    {
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null; //

        // 3. Emissão do Token JWT
        var claims = new[] { new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, "User") };

        // Define o segredo e algoritmo de assinatura
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Criação e escrita do token
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["Jwt:ExpiresInMinutes"]!)),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}