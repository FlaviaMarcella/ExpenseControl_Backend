namespace ExpenseControl.Api.Dto;

/// <summary>DTO de entrada para <c>POST /api/auth/login</c>.</summary>
/// <param name="Username">Nome de usuário cadastrado.</param>
/// <param name="Password">Senha em texto puro, verificada contra o hash salvo.</param>
public record LoginDto(string Username, string Password);