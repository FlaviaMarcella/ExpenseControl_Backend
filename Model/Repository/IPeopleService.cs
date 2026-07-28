using ExpenseControl.Api.Dto;

namespace ExpenseControl.Api.Model.Repository;

/// <summary>
/// Contrato para operações CRUD relacionadas à entidade "People" expostas pela camada de serviço.
/// </summary>
/// <remarks>
/// Implementações deste serviço devem mapear entre entidades de domínio e <see cref="PeopleDto"/>,
/// encapsulando lógica de persistência, validação e regras de negócio pertinentes.
/// Métodos retornam <see cref="Task"/> para suportar operações assíncronas I/O-bound.
/// </remarks>
public interface IPeopleService
{
    /// <summary>
    /// Recupera todas as pessoas disponíveis no sistema.
    /// </summary>
    /// <returns>
    /// Uma tarefa que produz uma sequência de <see cref="PeopleDto"/> representando todas as pessoas.
    /// </returns>
    Task<IEnumerable<PeopleDto>> GetAllAsync();

    /// <summary>
    /// Recupera uma pessoa pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador único da pessoa a ser buscada.</param>
    /// <returns>
    /// Uma tarefa que produz um <see cref="PeopleDto"/> quando encontrado, ou <c>null</c> se não existir.
    /// </returns>
    Task<PeopleDto?> GetByIdAsync(int id);

    /// <summary>
    /// Cria uma nova pessoa no sistema a partir dos dados fornecidos no DTO.
    /// </summary>
    /// <param name="peopleDto">DTO contendo os dados da pessoa a ser criada. O campo <c>Id</c> normalmente é ignorado/povoado pelo repositório.</param>
    /// <returns>
    /// Uma tarefa que produz o <see cref="PeopleDto"/> criado (incluindo o <c>Id</c> gerado).
    /// </returns>
    Task<PeopleDto> CreateAsync(PeopleDto peopleDto);

    /// <summary>
    /// Atualiza os dados de uma pessoa existente.
    /// </summary>
    /// <param name="id">Identificador da pessoa que será atualizada.</param>
    /// <param name="peopleDto">DTO contendo os novos valores que devem ser aplicados.</param>
    /// <returns>
    /// Uma tarefa que produz o <see cref="PeopleDto"/> atualizado, ou <c>null</c> se a pessoa com o <c>id</c> informado não existir.
    /// </returns>
    Task<PeopleDto?> UpdateAsync(int id, PeopleDto peopleDto);

    /// <summary>
    /// Remove uma pessoa do sistema.
    /// </summary>
    /// <param name="id">Identificador da pessoa a ser removida.</param>
    /// <returns>
    /// Uma tarefa que produz <c>true</c> se a exclusão ocorreu com sucesso, ou <c>false</c> se a pessoa não foi encontrada.
    /// </returns>
    Task<bool> DeleteAsync(int id);
}