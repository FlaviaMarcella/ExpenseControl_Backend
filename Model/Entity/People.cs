using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseControl.Api.Model.Enums;

namespace ExpenseControl.Api.Model.Entity;

public class People
{
    [Key]
    [Required(ErrorMessage = "Id is required.")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Age is required.")]
    public int Age { get; set; }

    public Relationship Relationship { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}