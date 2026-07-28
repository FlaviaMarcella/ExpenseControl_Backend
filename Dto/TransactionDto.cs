using ExpenseControl.Api.Model.Enums;

namespace ExpenseControl.Api.Dto;

/// <summary>
/// DTO (Data Transfer Object) imutável que representa uma transação no contexto da API ExpenseControl.
/// </summary>
/// <example>
/// Exemplo de criação:
/// <code>
/// var dto = new TransactionDto(
///     Id: 1,
///     Description: "Compra supermercado",
///     Amount: 123.45m,
///     Date: new DateOnly(2026, 7, 27),
///     Type: TypeTransaction.Expense,
///     People: new PeopleDto(1, "João", "Silva", 34, Relationship.Family, "joao@example.com", "+5511999999999")
/// );
/// </code>
/// </example>
/// <seealso cref="PeopleDto"/>
/// <param name="Id">Identificador único da transação (gerado pelo banco de dados).</param>
/// <param name="Description">Descrição resumida da transação.</param>
/// <param name="Amount">Valor monetário da transação.</param>
/// <param name="Date">Data da transação.</param>
/// <param name="Type">Tipo da transação (ex.: "Income", "Expense").</param>
/// <param name="People">Dados da pessoa associada à transação (<see cref="PeopleDto"/>).</param>
public record TransactionDto(
    int Id,
    string Description,
    decimal Amount,
    DateOnly Date,
    TypeTransaction Type,
    PeopleDto People);