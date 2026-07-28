using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseControl.Api.Model.Entity;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseControl.Api.Service;

/// <summary>
/// Responsável pela lógica de autenticação: hash de senha (BCrypt) e emissão de tokens JWT.
/// </summary>
/// <remarks>
/// As chaves de configuração usadas (<c>Jwt:Key</c>, <c>Jwt:Issuer</c>, <c>Jwt:Audience</c>,
/// <c>Jwt:ExpiresInMinutes</c>) devem existir em <c>appsettings.json</c> e são validadas
/// também em <c>Program.cs</c>, no registro do middleware de autenticação JWT.
/// </remarks>
public class AuthService(IConfiguration configuration)
{
    /// <summary>Gera o hash BCrypt de uma senha em texto puro, para ser persistido em <see cref="User.PasswordHash"/>.</summary>
    /// <param name="password">Senha em texto puro fornecida pelo usuário.</param>
    /// <returns>Hash BCrypt (já incluindo o salt), pronto para ser salvo no banco.</returns>
    public string RegisterPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    /// <summary>
    /// Verifica a senha informada contra o hash salvo e, se válida, gera um token JWT assinado.
    /// </summary>
    /// <param name="user">Usuário já carregado do banco (com <see cref="User.PasswordHash"/> preenchido).</param>
    /// <param name="password">Senha em texto puro informada no login.</param>
    /// <returns>O token JWT (string) se a senha for válida; <c>null</c> caso contrário.</returns>
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