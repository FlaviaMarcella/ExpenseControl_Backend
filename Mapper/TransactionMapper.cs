using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Model.Entity;

namespace ExpenseControl.Api.Mapper;

/// <summary>
/// Responsável por converter entre a entidade de domínio <see cref="Transaction"/> e o DTO <see cref="TransactionDto"/>.
/// </summary>
/// <remarks>
/// - Implementa mapeamentos manuais entre camadas (sem uso de bibliotecas externas como AutoMapper).
/// - Os métodos garantem que objetos obrigatórios não sejam nulos antes de realizar o mapeamento,
///   lançando <see cref="ArgumentNullException"/> em caso de falha.
/// </remarks>
public class TransactionMapper
{
    /// <summary>
    /// Converte uma entidade <see cref="Transaction"/> em um <see cref="TransactionDto"/>.
    /// </summary>
    /// <param name="transaction">A entidade de domínio a ser convertida. Não pode ser <c>null</c>.</param>
    /// <returns>
    /// Uma nova instância de <see cref="TransactionDto"/> preenchida com os dados da entidade.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Lançada quando <paramref name="transaction"/> ou <c>transaction.People</c> for <c>null</c>.
    /// </exception>
    public TransactionDto MapToDto(Transaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(transaction.People);

        var peopleMapper = new PeopleMapper();
        var peopleDto = peopleMapper.MapToDto(transaction.People);

        return new TransactionDto(
            transaction.Id,
            transaction.Description,
            transaction.Amount,
            transaction.Date,
            transaction.Type,
            peopleDto
        );
    }

    /// <summary>
    /// Converte um <see cref="TransactionDto"/> em uma entidade <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transactionDto">O DTO com os dados a serem convertidos. Não deve ser <c>null</c>.</param>
    /// <returns>
    /// Uma nova instância de <see cref="Transaction"/> preenchida com os valores do DTO.
    /// </returns>
    public Transaction MapToEntity(TransactionDto transactionDto)
    {
        var peopleMapper = new PeopleMapper();

        if (transactionDto.People == null)
        {
            throw new ArgumentNullException(nameof(transactionDto.People));
        }

        var people = peopleMapper.MapToEntity(transactionDto.People);


        return new Transaction
        {
            Id = transactionDto.Id,
            Amount = transactionDto.Amount,
            Date = transactionDto.Date,
            Description = transactionDto.Description,
            Type = transactionDto.Type,
            People = people
        };
    }
}