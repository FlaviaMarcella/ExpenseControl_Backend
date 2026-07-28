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
        var transactions = await context.Transactions
            .Include(t => t.People)
            .ToListAsync();
        return transactions.Select(transactionMapper.MapToDto);
    }

    public async Task<IEnumerable<TransactionDto>> GetByPeopleIdAsync(int peopleId)
    {
        var peopleExists = await peopleService.GetByIdAsync(peopleId);
        if (peopleExists == null)
        {
            throw new InvalidOperationException($"People with ID {peopleId} does not exist.");
        }

        var transactions = await context.Transactions
            .Include(t => t.People)
            .Where(t => t.People.Id == peopleId)
            .ToListAsync();
        return transactions.Select(transactionMapper.MapToDto);
    }

    public async Task<TransactionDto?> GetByIdAsync(int id)
    {
        var transaction = await context.Transactions
            .Include(t => t.People)
            .FirstOrDefaultAsync(t => t.Id == id);
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

        if (!TransactionRules.CanCreateReceiveTransaction(transactionDto.People.Age, transactionDto.Type))
        {
            throw new InvalidOperationException("People under 18 years old cannot create a receive transaction.");
        }

        var transaction = transactionMapper.MapToEntity(transactionDto);
        transaction.People = people;

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

        // Mesma regra de negócio aplicada na criação: menores de 18 anos só podem
        // ter transações de despesa. Sem essa checagem aqui, seria possível criar
        // uma despesa e depois "editá-la" para receita, contornando a regra.
        if (!TransactionRules.CanCreateReceiveTransaction(peopleExists.Age, transactionDto.Type))
        {
            throw new InvalidOperationException("People under 18 years old cannot create a receive transaction.");
        }

        transaction.Description = transactionDto.Description;
        transaction.Amount = transactionDto.Amount;
        transaction.Date = transactionDto.Date;
        transaction.Type = transactionDto.Type;
        transaction.People = (await context.Peoples.FindAsync(transactionDto.People.Id))!;
        await context.SaveChangesAsync();
        logger.LogInformation("Updated transaction with ID {Id}: {@Transaction}", id, transaction);
        return transactionMapper.MapToDto(transaction);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var transaction = await context.Transactions
            .Include(t => t.People)
            .FirstOrDefaultAsync(t => t.Id == id);
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

    /// <summary>
    /// Consulta de totais: percorre TODAS as pessoas cadastradas (não só as que têm
    /// transações) e, para cada uma, soma receitas e despesas separadamente para
    /// calcular o saldo. No final, agrega tudo num total geral.
    /// </summary>
    /// <remarks>
    /// Estratégia de implementação:
    /// 1. Carrega todas as pessoas com suas transações já incluídas (Include), evitando
    ///    o problema de N+1 queries que existiria se buscássemos as transações de cada
    ///    pessoa separadamente dentro do loop.
    /// 2. Para cada pessoa, agrupa as transações por tipo (Receive/Expense) e soma os
    ///    valores em memória — o volume de dados de um controle de gastos doméstico é
    ///    pequeno o suficiente para isso ser tranquilo sem precisar de agregação no banco.
    /// 3. Os totais gerais são a soma simples dos totais individuais já calculados,
    ///    evitando percorrer a lista de transações uma segunda vez.
    /// </remarks>
    public async Task<TotalsResponseDto> GetTotalsAsync()
    {
        var peoples = await context.Peoples
            .Include(p => p.Transactions)
            .ToListAsync();

        var personTotals = peoples.Select(p =>
        {
            var income = p.Transactions.Where(t => t.Type == TypeTransaction.Receive).Sum(t => t.Amount);
            var expense = p.Transactions.Where(t => t.Type == TypeTransaction.Expense).Sum(t => t.Amount);

            return new PersonTotalsDto(
                PeopleId: p.Id,
                Name: $"{p.Name} {p.LastName}",
                TotalIncome: income,
                TotalExpense: expense,
                Balance: income - expense);
        }).ToList();

        var grandIncome = personTotals.Sum(p => p.TotalIncome);
        var grandExpense = personTotals.Sum(p => p.TotalExpense);

        logger.LogInformation(
            "Calculated totals for {Count} people. Grand income: {Income}, grand expense: {Expense}",
            personTotals.Count, grandIncome, grandExpense);

        return new TotalsResponseDto(
            People: personTotals,
            GrandTotalIncome: grandIncome,
            GrandTotalExpense: grandExpense,
            GrandBalance: grandIncome - grandExpense);
    }
}