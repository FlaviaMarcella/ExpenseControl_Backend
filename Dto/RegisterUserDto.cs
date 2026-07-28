namespace ExpenseControl.Api.Dto;

/// <summary>
/// DTO de entrada para <c>POST /api/auth/register</c>. Distinto de <see cref="UserDto"/>
/// (que é a saída) justamente para que a senha em texto puro nunca seja ecoada na resposta.
/// </summary>
/// <param name="Username">Nome de usuário desejado (deve ser único).</param>
/// <param name="Password">Senha em texto puro; será convertida em hash BCrypt antes de salvar.</param>
/// <param name="PeopleId">
/// Identificador de uma <see cref="Model.Entity.People"/> já existente para associar ao usuário,
/// ou <c>null</c> para criar um usuário sem pessoa vinculada (ex.: conta administrativa).
/// </param>
public record RegisterUserDto(string Username, string Password, int? PeopleId);