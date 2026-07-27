using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Model.Repository;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExpenseControl.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Transaction.Base)]
[SwaggerTag("Transações")]
public class TransactionController(ITransactionService transactionService, IPeopleService peopleService)
    : ControllerBase
{
    [SwaggerOperation(
        Summary = "Get all transactions"
    )]
    [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll()
    {
        var transactions = await transactionService.GetAllAsync();
        return Ok(transactions);
    }

    [SwaggerOperation(
        Summary = "Get transactions by people ID"
    )]
    [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet($"{ApiRoutes.Transaction.GetByPeopleId}/{{peopleId}}")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByPeopleId(int peopleId)
    {
        var people = await peopleService.GetByIdAsync(peopleId);
        if (people == null)
        {
            return NotFound();
        }

        var transactions = await transactionService.GetByPeopleIdAsync(peopleId);
        return Ok(transactions);
    }

    [SwaggerOperation(
        Summary = "Get a transaction by ID"
    )]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetById(int id)
    {
        var transaction = await transactionService.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }

    [SwaggerOperation(
        Summary = "Create a new transaction"
    )]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(TransactionDto transactionDto)
    {
        var people = await peopleService.GetByIdAsync(transactionDto.People.Id);
        if (people == null)
        {
            return NotFound($"People with ID {transactionDto.People.Id} does not exist.");
        }

        var created = await transactionService.CreateAsync(transactionDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [SwaggerOperation(
        Summary = "Update an existing transaction"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("{id}")]
    public async Task<ActionResult<TransactionDto>> Update(int id, TransactionDto transactionDto)
    {
        var updated = await transactionService.UpdateAsync(id, transactionDto);
        if (updated == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [SwaggerOperation(
        Summary = "Delete a transaction"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var transaction = await transactionService.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }

        await transactionService.DeleteAsync(id);
        return NoContent();
    }

    [SwaggerOperation(
        Summary = "Delete all transactions by people ID"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete($"{ApiRoutes.Transaction.GetByPeopleId}/{{peopleId}}")]
    public async Task<ActionResult> DeleteAllByPeopleId(int peopleId)
    {
        var people = await peopleService.GetByIdAsync(peopleId);
        if (people == null)
        {
            return NotFound();
        }

        await transactionService.DeleteAllByPeopleIdAsync(peopleId);
        return NoContent();
    }
}