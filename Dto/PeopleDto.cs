using ExpenseControl.Api.Model.Enums;

namespace ExpenseControl.Api.Dto;

/// <summary>
/// DTO (Data Transfer Object) imutável que representa uma pessoa no contexto da API ExpenseControl.
/// </summary>
/// <remarks>
/// - Este record é usado para transportar dados entre camadas (por exemplo: Controllers → Services → Clients).
/// - É um record posicional: os parâmetros do construtor geram automaticamente propriedades imutáveis (init-only).
/// - As propriedades de texto (<c>Name</c>, <c>LastName</c>, <c>Email</c>, <c>Phone</c>) são declaradas como não-nulas; 
/// </remarks>
/// <example>
/// Exemplo de criação:
/// <code>
/// var dto = new PeopleDto(
///     Id: 1,
///     Name: "João",
///     LastName: "Silva",
///     DateOfBirth: new DateOnly(1990, 5, 15),
///     Age: 34,
///     Relationship: Relationship.Family,
///     Email: "joao.silva@example.com",
///     Phone: "+5511999999999");
/// </code>
/// </example>
/// <seealso cref="ExpenseControl.Api.Model.Entity.People"/>
public record PeopleDto(
    int Id,
    string Name,
    string LastName,
    DateOnly DateOfBirth,
    int Age,
    Relationship Relationship,
    string Email,
    string Phone);