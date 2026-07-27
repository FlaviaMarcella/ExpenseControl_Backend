using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Model.Entity;

namespace ExpenseControl.Api.Mapper;

public class PeopleMapper
{
    public PeopleDto MapToDto(People people)
    {
        ArgumentNullException.ThrowIfNull(people);

        return new PeopleDto(people.Id, people.Name, people.LastName, people.Age, people.Relationship, people.Email,
            people.Phone);
    }

    public People MapToEntity(PeopleDto peopleDto)
    {
        return new People
        {
            Id = peopleDto.Id,
            Name = peopleDto.Name,
            LastName = peopleDto.LastName,
            Age = peopleDto.Age,
            Relationship = peopleDto.Relationship,
            Email = peopleDto.Email,
            Phone = peopleDto.Phone
        };
    }
}