using ExpenseControl.Api.Data;
using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Mapper;
using ExpenseControl.Api.Model.Domain;
using ExpenseControl.Api.Model.Enums;
using ExpenseControl.Api.Model.Repository;
using Microsoft.EntityFrameworkCore;

namespace ExpenseControl.Api.Service;

public class TransactionService(
    AppDbContext context,
    IPeopleService peopleService,
    ILogger<TransactionService> logger,
    TransactionMapper transactionMapper) : ITransactionService
{
    public async Task<IEnumerable<TransactionDto>> GetAllAsync()
    {
        var transactions = await context.Transactions.ToListAsync();
        return transactions.Select(transactionMapper.MapToDto);
    }

    public async Task<IEnumerable<TransactionDto>> GetByPeopleIdAsync(int peopleId)
    {
        var peopleExists = await peopleService.GetByIdAsync(peopleId);
        if (peopleExists == null)
        {
            throw new InvalidOperationException($"People with ID {peopleId} does not exist.");
        }

        var transactions = await context.Transactions.Where(t => t.People.Id == peopleId).ToListAsync();
        return transactions.Select(transactionMapper.MapToDto);
    }

    public async Task<TransactionDto?> GetByIdAsync(int id)
    {
        var transaction = await context.Transactions.FindAsync(id);
        if (transaction == null)
        {
            return null;
        }

        logger.LogInformation("Retrieved transaction with ID {Id}: {@Transaction}", id, transaction);
        return transactionMapper.MapToDto(transaction);
    }

    public async Task<TransactionDto> CreateAsync(TransactionDto transactionDto)
    {
        var people = await context.Peoples.FindAsync(transactionDto.People.Id);
        if (people == null)
        {
            throw new InvalidOperationException($"People with ID {transactionDto.People.Id} does not exist.");
        }

        var transaction = transactionMapper.MapToEntity(transactionDto);
        transaction.People = people;

        if (!TransactionRules.CanCreateReceiveTransaction(transaction.People.Age, transaction.Type))
        {
            throw new InvalidOperationException("People under 18 years old cannot create a receive transaction.");
        }

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();
        logger.LogInformation("Created new transaction: {@Transaction}", transaction);
        return transactionMapper.MapToDto(transaction);
    }

    public async Task<TransactionDto?> UpdateAsync(int id, TransactionDto transactionDto)
    {
        var transaction = await context.Transactions.FindAsync(id);
        if (transaction == null)
        {
            return null;
        }

        var peopleExists = await peopleService.GetByIdAsync(transactionDto.People.Id);
        if (peopleExists == null)
        {
            throw new InvalidOperationException($"People with ID {transactionDto.People.Id} does not exist.");
        }

        transaction.Description = transactionDto.Description;
        transaction.Amount = transactionDto.Amount;
        transaction.Date = transactionDto.Date;
        transaction.Type = Enum.Parse<TypeTransaction>(transactionDto.Type);
        transaction.People = (await context.Peoples.FindAsync(transactionDto.People.Id))!;
        await context.SaveChangesAsync();
        logger.LogInformation("Updated transaction with ID {Id}: {@Transaction}", id, transaction);
        return transactionMapper.MapToDto(transaction);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var transaction = await context.Transactions.FindAsync(id);
        if (transaction == null)
        {
            return false;
        }

        context.Transactions.Remove(transaction);
        await context.SaveChangesAsync();
        logger.LogInformation("Deleted transaction with ID {Id}: {@Transaction}", id, transaction);
        return true;
    }


    public async Task<bool> DeleteAllByPeopleIdAsync(int peopleId)
    {
        var peopleExists = await peopleService.GetByIdAsync(peopleId);
        if (peopleExists == null)
        {
            throw new InvalidOperationException($"People with ID {peopleId} does not exist.");
        }

        var transactions = await context.Transactions.Where(t => t.People.Id == peopleId).ToListAsync();
        if (!transactions.Any())
        {
            return false;
        }

        context.Transactions.RemoveRange(transactions);
        await context.SaveChangesAsync();
        logger.LogInformation("Deleted all transactions for people with ID {PeopleId}", peopleId);
        return true;
    }
}