using ExpenseControl.Api.Model.Enums;

namespace ExpenseControl.Api.Dto;

public record PeopleDto(
    int Id,
    string Name,
    string LastName,
    int Age,
    Relationship Relationship,
    string Email,
    string Phone);