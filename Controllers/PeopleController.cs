using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Model.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl.Api.Controllers;

[ApiController]
[Route(ApiRoutes.People.Base)]
public class PeopleController(IPeopleService peopleService, ITransactionService transactionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PeopleDto>>> GetAll()
    {
        var peoples = await peopleService.GetAllAsync();
        return Ok(peoples);
    }

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

    [HttpPost]
    public async Task<ActionResult<PeopleDto>> Create(PeopleDto peopleDto)
    {
        var created = await peopleService.CreateAsync(peopleDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

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