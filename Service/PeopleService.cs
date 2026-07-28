using ExpenseControl.Api.Data;
using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Mapper;
using ExpenseControl.Api.Model.Repository;
using Microsoft.EntityFrameworkCore;

namespace ExpenseControl.Api.Service;

/// <summary>
/// Implementação de <see cref="IPeopleService"/> usando o EF Core (<see cref="AppDbContext"/>)
/// diretamente como camada de acesso a dados (sem Repository intermediário).
/// </summary>
public class PeopleService(
    AppDbContext context,
    ILogger<PeopleService> logger,
    PeopleMapper peopleMapper) : IPeopleService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<PeopleDto>> GetAllAsync()
    {
        var peoples = await context.Peoples.ToListAsync();
        return peoples.Select(p => peopleMapper.MapToDto(p));
    }

    /// <inheritdoc/>
    public async Task<PeopleDto?> GetByIdAsync(int id)
    {
        var people = await context.Peoples.FindAsync(id);
        if (people == null)
        {
            return null;
        }

        logger.LogInformation("Retrieved people with ID {Id}: {@People}", id, people);
        return peopleMapper.MapToDto(people);
    }

    /// <inheritdoc/>
    public async Task<PeopleDto> CreateAsync(PeopleDto peopleDto)
    {
        var people = peopleMapper.MapToEntity(peopleDto);

        context.Peoples.Add(people);
        await context.SaveChangesAsync();
        logger.LogInformation("Created new people: {@People}", people);
        return peopleMapper.MapToDto(people);
    }

    /// <inheritdoc/>
    public async Task<PeopleDto?> UpdateAsync(int id, PeopleDto peopleDto)
    {
        var people = await context.Peoples.FindAsync(id);
        if (people == null)
        {
            return null;
        }

        people.Name = peopleDto.Name;
        people.LastName = peopleDto.LastName;
        people.BirthDate = peopleDto.DateOfBirth;
        people.Relationship = peopleDto.Relationship;
        people.Email = peopleDto.Email;
        people.Phone = peopleDto.Phone;

        await context.SaveChangesAsync();
        logger.LogInformation("Updated people with ID {Id}: {@People}", id, people);
        return peopleMapper.MapToDto(people);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var people = await context.Peoples.FindAsync(id);
        if (people == null)
        {
            return false;
        }

        context.Peoples.Remove(people);
        await context.SaveChangesAsync();
        logger.LogInformation("Deleted people with ID {Id}: {@People}", id, people);
        return true;
    }
}