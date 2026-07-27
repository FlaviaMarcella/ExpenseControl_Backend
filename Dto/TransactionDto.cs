namespace ExpenseControl.Api.Dto;

public record TransactionDto(int Id, string Description, decimal Amount, DateOnly Date, string Type, PeopleDto People);