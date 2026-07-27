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

    [Required(ErrorMessage = "Last Name is required.")]
    [MaxLength(150, ErrorMessage = "Last Name cannot exceed 150 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth Date is required.")]
    public DateOnly BirthDate { get; set; }

    public Relationship Relationship { get; set; }

    [EmailAddress(ErrorMessage = "Email is invalid.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Phone is invalid")]
    public string Phone { get; set; } = string.Empty;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}