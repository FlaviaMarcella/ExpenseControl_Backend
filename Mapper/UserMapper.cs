using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Model.Entity;

namespace ExpenseControl.Api.Mapper;

public class UserMapper(PeopleMapper peopleMapper)
{
    public UserDto MapToDto(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var peopleDto = user.People != null ? peopleMapper.MapToDto(user.People) : null;
        return new UserDto(user.Id, user.Username, peopleDto);
    }
}