using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Model.Entity;

namespace ExpenseControl.Api.Mapper;

/// <summary>
/// Responsável por converter <see cref="User"/> em <see cref="UserDto"/> para resposta da API.
/// </summary>
/// <remarks>
/// Não possui (nem deveria possuir) um <c>MapToEntity</c>: montar um <see cref="User"/> a partir
/// dos dados de registro exige hash de senha (<see cref="Service.AuthService"/>) e busca da
/// <see cref="People"/> existente no banco — responsabilidades que pertencem ao Controller/Service,
/// não a um Mapper puro.
/// </remarks>
public class UserMapper(PeopleMapper peopleMapper)
{
    /// <summary>
    /// Converte um <see cref="User"/> em <see cref="UserDto"/>. Nunca inclui a senha/hash na saída.
    /// </summary>
    /// <exception cref="ArgumentNullException">Lançada quando <paramref name="user"/> for <c>null</c>.</exception>
    public UserDto MapToDto(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var peopleDto = user.People != null ? peopleMapper.MapToDto(user.People) : null;
        return new UserDto(user.Id, user.Username, peopleDto);
    }
}