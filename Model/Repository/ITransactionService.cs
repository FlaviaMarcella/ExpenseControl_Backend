using ExpenseControl.Api.Dto;

namespace ExpenseControl.Api.Model.Repository;

/// <summary>
/// Contrato para operações CRUD e de consulta relacionadas a <see cref="Entity.Transaction"/>.
/// </summary>
/// <remarks>
/// Implementações validam a existência da <see cref="Entity.People"/> associada e aplicam
/// as regras de negócio de <see cref="Domain.TransactionRules"/> antes de persistir.
/// </remarks>
public interface ITransactionService
{
    /// <summary>Recupera todas as transações cadastradas no sistema.</summary>
    Task<IEnumerable<TransactionDto>> GetAllAsync();

    /// <summary>
    /// Recupera todas as transações associadas a uma pessoa específica.
    /// </summary>
    /// <param name="peopleId">Identificador da pessoa.</param>
    /// <exception cref="InvalidOperationException">Lançada se a pessoa não existir.</exception>
    Task<IEnumerable<TransactionDto>> GetByPeopleIdAsync(int peopleId);

    /// <summary>Recupera uma transação pelo seu identificador, ou <c>null</c> se não existir.</summary>
    Task<TransactionDto?> GetByIdAsync(int id);

    /// <summary>
    /// Cria uma nova transação, validando que a pessoa associada existe e que a regra
    /// de idade mínima para receitas é respeitada.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Lançada se a pessoa não existir, ou se a transação violar <see cref="Domain.TransactionRules"/>.
    /// </exception>
    Task<TransactionDto> CreateAsync(TransactionDto transactionDto);

    /// <summary>Atualiza uma transação existente, ou retorna <c>null</c> se o <paramref name="id"/> não existir.</summary>
    Task<TransactionDto?> UpdateAsync(int id, TransactionDto transactionDto);

    /// <summary>Remove uma transação. Retorna <c>false</c> se o <paramref name="id"/> não existir.</summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Remove todas as transações associadas a uma pessoa (usado ao excluir a própria pessoa,
    /// para evitar violação de integridade referencial).
    /// </summary>
    /// <returns><c>true</c> se havia alguma transação a excluir; <c>false</c> se a pessoa não tinha nenhuma.</returns>
    Task<bool> DeleteAllByPeopleIdAsync(int peopleId);
}