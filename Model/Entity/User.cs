using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseControl.Api.Model.Entity;

/// <summary>
/// Entidade de domínio que representa uma credencial de acesso ao sistema (login).
/// </summary>
/// <remarks>
/// Conceito distinto de <see cref="People"/>: um <see cref="User"/> é quem consegue autenticar
/// na API; uma <see cref="People"/> é um membro da família cujos gastos são controlados.
/// A associação entre os dois é opcional — nem todo usuário (ex.: uma conta administrativa)
/// precisa representar uma pessoa real da família.
/// </remarks>
public class User
{
    /// <summary>Identificador único, gerado automaticamente pelo banco de dados.</summary>
    [Key]
    [Required(ErrorMessage = "Id is required.")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>Nome de usuário único, utilizado para login.</summary>
    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Hash BCrypt da senha (nunca a senha em texto puro). Gerado por
    /// <see cref="Service.AuthService.RegisterPassword"/> no momento do registro.
    /// </summary>
    [Required(ErrorMessage = "Password hash is required.")]
    [MaxLength(100)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Pessoa opcionalmente associada a este usuário. <c>null</c> para contas que não
    /// representam um membro específico da família (ex.: uma conta administrativa).
    /// </summary>
    public People? People { get; set; }
}