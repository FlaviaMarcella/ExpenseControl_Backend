using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseControl.Api.Model.Entity;

public class User
{
    [Key]
    [Required(ErrorMessage = "Id is required.")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password hash is required.")]
    [MaxLength(100)]
    public string PasswordHash { get; set; } = string.Empty;

    public People? People { get; set; }
}