using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseControl.Api.Model.Enums;

namespace ExpenseControl.Api.Model.Entity;

/// <summary>
/// Entidade de domínio que representa um membro da família cujas transações são controladas pelo sistema.
/// </summary>
/// <remarks>
/// Mapeada pelo EF Core para a tabela "People". A idade não é persistida como coluna —
/// é sempre calculada a partir de <see cref="BirthDate"/> via <see cref="Domain.DateUtils.CalculateAge"/>,
/// evitando que o dado fique desatualizado com o passar do tempo.
/// </remarks>
public class People
{
    /// <summary>Identificador único, gerado automaticamente pelo banco de dados.</summary>
    [Key]
    [Required(ErrorMessage = "Id is required.")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>Primeiro nome da pessoa.</summary>
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Sobrenome da pessoa.</summary>
    [Required(ErrorMessage = "Last Name is required.")]
    [MaxLength(150, ErrorMessage = "Last Name cannot exceed 150 characters.")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>Data de nascimento, usada para calcular a idade sob demanda.</summary>
    [Required(ErrorMessage = "Birth Date is required.")]
    public DateOnly BirthDate { get; set; }

    /// <summary>Grau de parentesco desta pessoa com o núcleo familiar (ver <see cref="Enums.Relationship"/>).</summary>
    public Relationship Relationship { get; set; }

    /// <summary>Endereço de e-mail para contato.</summary>
    [EmailAddress(ErrorMessage = "Email is invalid.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Número de telefone para contato.</summary>
    [Phone(ErrorMessage = "Phone is invalid")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Transações associadas a esta pessoa. Ao excluir uma <see cref="People"/>, as transações
    /// relacionadas devem ser removidas antes (ver <c>TransactionService.DeleteAllByPeopleIdAsync</c>).
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}