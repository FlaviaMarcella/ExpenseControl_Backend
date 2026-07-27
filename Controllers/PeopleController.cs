using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Model.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExpenseControl.Api.Controllers;

[Authorize]
[ApiController]
[Route(ApiRoutes.People.Base)]
public class PeopleController(IPeopleService peopleService, ITransactionService transactionService) : ControllerBase
{
    /// <summary>
    /// Retorna todas as pessoas cadastradas no sistema.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Retorna todas as pessoas cadastradas no sistema",
        Description =
            "Retorna uma lista de todas as pessoas cadastradas no sistema, incluindo seus identificadores únicos e demais informações relevantes.")]
    [SwaggerResponse(200, "Lista de pessoas retornada com sucesso", typeof(IEnumerable<PeopleDto>))]
    public async Task<ActionResult<IEnumerable<PeopleDto>>> GetAll()
    {
        var peoples = await peopleService.GetAllAsync();
        return Ok(peoples);
    }

    /// <summary>
    /// Busca uma pessoa pelo seu identificador único.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Busca pessoa por Id",
        Description = "Retorna os dados de uma pessoa cadastrada, ou 404 caso não exista.")]
    [SwaggerResponse(200, "Pessoa encontrada", typeof(PeopleDto))]
    [SwaggerResponse(404, "Pessoa não encontrada")]
    public async Task<ActionResult<PeopleDto>> GetById(int id)
    {
        var people = await peopleService.GetByIdAsync(id);
        if (people == null)
        {
            return NotFound();
        }

        return Ok(people);
    }

    /// <summary>
    /// Cria uma nova pessoa com os dados fornecidos.
    /// </summary>
    /// <param name="peopleDto"></param>
    /// <returns></returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cria uma nova pessoa",
        Description =
            "Cria uma nova pessoa com os dados fornecidos e retorna o objeto criado com seu identificador único.")]
    [SwaggerResponse(200, "Pessoa criada com sucesso", typeof(PeopleDto))]
    public async Task<ActionResult<PeopleDto>> Create(PeopleDto peopleDto)
    {
        var created = await peopleService.CreateAsync(peopleDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza os dados de uma pessoa já existente com os dados fornecidos.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="peopleDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualiza os dados de uma pessoa, já existente",
        Description = "Atualiza os dados de uma pessoa já existente com os dados fornecidos.")
    ]
    [SwaggerResponse(200, "Pessoa atualizada com sucesso", typeof(PeopleDto))]
    [SwaggerResponse(404, "Pessoa não encontrada")]
    public async Task<ActionResult<PeopleDto>> Update(int id, PeopleDto peopleDto)
    {
        var updated = await peopleService.UpdateAsync(id, peopleDto);
        if (updated == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Exclui uma pessoa pelo seu identificador único, juntamente com todas as transações associadas a ela.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Exclui uma pessoa",
        Description =
            "Exclui uma pessoa pelo seu identificador único, juntamente com todas as transações associadas a ela."
    )]
    [SwaggerResponse(204, "Pessoa excluída com sucesso")]
    [SwaggerResponse(404, "Pessoa não encontrada")]
    public async Task<ActionResult> Delete(int id)
    {
        var people = await peopleService.GetByIdAsync(id);
        if (people == null)
        {
            return NotFound();
        }

        await transactionService.DeleteAllByPeopleIdAsync(id); // Delete all transactions associated with the person
        await peopleService.DeleteAsync(id);
        return NoContent();
    }
}