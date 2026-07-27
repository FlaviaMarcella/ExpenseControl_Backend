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
}