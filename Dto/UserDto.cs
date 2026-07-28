namespace ExpenseControl.Api.Dto;

/// <summary>
/// DTO de saída representando um usuário autenticável. Nunca contém senha ou hash —
/// apenas dados seguros de exibir ao cliente da API.
/// </summary>
/// <param name="Id">Identificador único do usuário.</param>
/// <param name="Username">Nome de usuário usado para login.</param>
/// <param name="People">Pessoa associada, ou <c>null</c> se o usuário não representa um membro específico da família.</param>
public record UserDto(
    int Id,
    string Username,
    PeopleDto? People
);