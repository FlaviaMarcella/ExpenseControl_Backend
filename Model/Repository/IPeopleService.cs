using ExpenseControl.Api.Dto;

namespace ExpenseControl.Api.Model.Repository;

public interface IPeopleService
{
    Task<IEnumerable<PeopleDto>> GetAllAsync();
    Task<PeopleDto?> GetByIdAsync(int id);
    Task<PeopleDto> CreateAsync(PeopleDto peopleDto);
    Task<PeopleDto?> UpdateAsync(int id, PeopleDto peopleDto);
    Task<bool> DeleteAsync(int id);
}