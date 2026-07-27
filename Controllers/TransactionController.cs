using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Model.Repository;

namespace ExpenseControl.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Transaction.Base)]
public class TransactionController(ITransactionService transactionService, IPeopleService peopleService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll()
    {
        var transactions = await transactionService.GetAllAsync();
        return Ok(transactions);
    }

    [HttpGet("/{peopleId}")]
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

    [HttpDelete]
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

    [HttpDelete("{peopleId}")]
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