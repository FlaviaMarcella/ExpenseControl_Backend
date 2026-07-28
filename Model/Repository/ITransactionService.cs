using ExpenseControl.Api.Dto;

namespace ExpenseControl.Api.Model.Repository;

public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetAllAsync();
    Task<IEnumerable<TransactionDto>> GetByPeopleIdAsync(int peopleId);
    Task<TransactionDto?> GetByIdAsync(int id);
    Task<TransactionDto> CreateAsync(TransactionDto transactionDto);
    Task<TransactionDto?> UpdateAsync(int id, TransactionDto transactionDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteAllByPeopleIdAsync(int peopleId);

    /// <summary>
    /// Calcula a "Consulta de totais": para cada pessoa cadastrada, soma suas receitas,
    /// despesas e o saldo resultante; ao final, soma tudo num total geral.
    /// Pessoas sem nenhuma transação aparecem na lista com todos os totais zerados
    /// (a consulta é sobre PESSOAS cadastradas, não sobre transações existentes).
    /// </summary>
    Task<TotalsResponseDto> GetTotalsAsync();
}