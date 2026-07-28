namespace ExpenseControl.Api.Dto;

/// <summary>
/// Totais consolidados de uma única pessoa: soma de todas as transações do tipo
/// "Receive" (receitas), soma de todas do tipo "Expense" (despesas) e o saldo
/// resultante (receitas - despesas).
/// </summary>
/// <param name="PeopleId">Identificador da pessoa a que estes totais pertencem.</param>
/// <param name="Name">Nome completo da pessoa (para exibição direta na listagem).</param>
/// <param name="TotalIncome">Soma de todas as transações do tipo receita.</param>
/// <param name="TotalExpense">Soma de todas as transações do tipo despesa.</param>
/// <param name="Balance">Saldo da pessoa: <c>TotalIncome - TotalExpense</c>.</param>
public record PersonTotalsDto(
    int PeopleId,
    string Name,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Balance);

/// <summary>
/// Resposta completa da "Consulta de totais": os totais de cada pessoa cadastrada,
/// mais o total geral (soma de todas as pessoas) ao final — exatamente como pedido
/// na especificação: "deverá listar todas as pessoas... e ao final... o total geral".
/// </summary>
/// <param name="People">Totais individuais, uma entrada por pessoa cadastrada.</param>
/// <param name="GrandTotalIncome">Soma das receitas de todas as pessoas.</param>
/// <param name="GrandTotalExpense">Soma das despesas de todas as pessoas.</param>
/// <param name="GrandBalance">Saldo líquido geral: <c>GrandTotalIncome - GrandTotalExpense</c>.</param>
public record TotalsResponseDto(
    IEnumerable<PersonTotalsDto> People,
    decimal GrandTotalIncome,
    decimal GrandTotalExpense,
    decimal GrandBalance);