namespace ExpenseControl.Api.Dto;

public record UserDto(
    int Id,
    string Username,
    PeopleDto? People
);