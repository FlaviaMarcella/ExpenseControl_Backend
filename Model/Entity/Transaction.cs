using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseControl.Api.Model.Enums;

namespace ExpenseControl.Api.Model.Entity;

public class Transaction
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
    [Required]
    public string Description { get; set; } = string.Empty;

    [Required] public decimal Amount { get; set; }

    [Required] public DateOnly Date { get; set; }

    [Required] public TypeTransaction Type { get; set; }

    public People People { get; set; } = null!;
}