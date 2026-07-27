using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Model.Entity;
using ExpenseControl.Api.Model.Enums;

namespace ExpenseControl.Api.Mapper;

public class TransactionMapper
{
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
            transaction.Type.ToString(),
            peopleDto
        );
    }

    public Transaction MapToEntity(TransactionDto transactionDto)
    {
        var peopleMapper = new PeopleMapper();
        var people = peopleMapper.MapToEntity(transactionDto.People);

        return new Transaction
        {
            Id = transactionDto.Id,
            Amount = transactionDto.Amount,
            Date = transactionDto.Date,
            Description = transactionDto.Description,
            Type = Enum.Parse<TypeTransaction>(transactionDto.Type),
            People = people
        };
    }
}