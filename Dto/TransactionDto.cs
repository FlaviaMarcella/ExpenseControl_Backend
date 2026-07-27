namespace ExpenseControl.Api.Dto;

public record TransactionDto(int id, string description, decimal amount, DateOnly date, string type, PeopleDto people);