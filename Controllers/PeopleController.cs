using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Model.Repository;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExpenseControl.Api.Controllers;

[ApiController]
[Route(ApiRoutes.People.Base)]
public class PeopleController(IPeopleService peopleService, ITransactionService transactionService) : ControllerBase
{
    [SwaggerOperation(
        Summary = "Get all people"
    )]
    [ProducesResponseType(typeof(IEnumerable<PeopleDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PeopleDto>>> GetAll()
    {
        var peoples = await peopleService.GetAllAsync();
        return Ok(peoples);
    }

    [SwaggerOperation(
        Summary = "Get a person by ID"
    )]
    [ProducesResponseType(typeof(PeopleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    public async Task<ActionResult<PeopleDto>> GetById(int id)
    {
        var people = await peopleService.GetByIdAsync(id);
        if (people == null)
        {
            return NotFound();
        }

        return Ok(people);
    }

    [SwaggerOperation(
        Summary = "Create a new person"
    )]
    [ProducesResponseType(typeof(PeopleDto), StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<ActionResult<PeopleDto>> Create(PeopleDto peopleDto)
    {
        var created = await peopleService.CreateAsync(peopleDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [SwaggerOperation(
        Summary = "Update an existing person"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id}")]
    public async Task<ActionResult<PeopleDto>> Update(int id, PeopleDto peopleDto)
    {
        var updated = await peopleService.UpdateAsync(id, peopleDto);
        if (updated == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [SwaggerOperation(
        Summary = "Delete a person"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id}")]
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