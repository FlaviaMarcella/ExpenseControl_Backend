using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Model.Domain;
using ExpenseControl.Api.Model.Entity;

namespace ExpenseControl.Api.Mapper;

/// <summary>
/// Responsável por converter entre a entidade de domínio <see cref="People"/> e o DTO <see cref="PeopleDto"/>.
/// </summary>
/// <remarks>
/// Implementa mapeamentos manuais (sem bibliotecas externas) usados pela camada de serviço/controllers.
/// </remarks>
public class PeopleMapper
{
    /// <summary>
    /// Converte uma instância de <see cref="People"/> para <see cref="PeopleDto"/>.
    /// </summary>
    /// <param name="people">A entidade de domínio a ser convertida. Não pode ser <c>null</c>.</param>
    /// <returns>Uma nova instância de <see cref="PeopleDto"/> contendo os dados mapeados.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando <paramref name="people"/> for <c>null</c>.</exception>
    public PeopleDto MapToDto(People people)
    {
        ArgumentNullException.ThrowIfNull(people);

        return new PeopleDto(people.Id, people.Name, people.LastName, people.BirthDate,
            DateUtils.CalculateAge(people.BirthDate), people.Relationship, people.Email,
            people.Phone);
    }

    /// <summary>
    /// Converte um <see cref="PeopleDto"/> em uma entidade <see cref="People"/>.
    /// </summary>
    /// <param name="peopleDto">O DTO que contém os dados a serem convertidos.</param>
    /// <returns>Uma nova instância de <see cref="People"/> preenchida com os valores do DTO.</returns>
    public People MapToEntity(PeopleDto peopleDto)
    {
        return new People
        {
            Id = peopleDto.Id,
            Name = peopleDto.Name,
            LastName = peopleDto.LastName,
            BirthDate = peopleDto.DateOfBirth,
            Relationship = peopleDto.Relationship,
            Email = peopleDto.Email,
            Phone = peopleDto.Phone
        };
    }
}