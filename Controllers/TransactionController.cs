using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Model.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExpenseControl.Api.Controllers;

[Authorize]
[ApiController]
[Route(ApiRoutes.Transaction.Base)]
[SwaggerTag("Transações")]
public class TransactionController(ITransactionService transactionService, IPeopleService peopleService)
    : ControllerBase
{
    /// <summary>
    /// Retorna todas as transações cadastradas no sistema.
    /// </summary>
    /// <returns></returns>
    [SwaggerOperation(
        Summary = "Retorna todas as transações cadastradas no sistema",
        Description =
            "Retorna todas as transações cadastradas no sistema, incluindo informações sobre a pessoa associada a cada transação."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Transações retornadas com sucesso", typeof(IEnumerable<TransactionDto>))]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll()
    {
        var transactions = await transactionService.GetAllAsync();
        return Ok(transactions);
    }

    /// <summary>
    /// Consulta de totais: lista todas as pessoas cadastradas com seus totais de
    /// receitas, despesas e saldo, e ao final devolve o total geral (soma de todas
    /// as pessoas). Atende diretamente ao requisito de "Consulta de totais" da
    /// especificação do sistema.
    /// </summary>
    /// <remarks>
    /// Observação de rota: como este método usa um segmento literal ("totals"),
    /// o ASP.NET Core o resolve com prioridade sobre a rota parametrizada
    /// GET /api/transaction/{id} — não há ambiguidade entre as duas.
    /// </remarks>
    [SwaggerOperation(
        Summary = "Consulta de totais por pessoa e total geral",
        Description =
            "Retorna, para cada pessoa cadastrada, o total de receitas, o total de despesas e o saldo " +
            "(receita - despesa). Ao final, retorna também o total geral somando todas as pessoas.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Totais calculados com sucesso", typeof(TotalsResponseDto))]
    [HttpGet(ApiRoutes.Transaction.Totals)]
    public async Task<ActionResult<TotalsResponseDto>> GetTotals()
    {
        var totals = await transactionService.GetTotalsAsync();
        return Ok(totals);
    }

    /// <summary>
    /// Retorna todas as transações associadas a uma pessoa específica.
    /// </summary>
    /// <param name="peopleId"></param>
    /// <returns></returns>
    [HttpGet($"{ApiRoutes.Transaction.GetByPeopleId}/{{peopleId}}")]
    [SwaggerOperation(
        Summary = "Retorna todas as transações associadas a uma pessoa específica",
        Description =
            "Retorna todas as transações associadas a uma pessoa específica, identificada pelo seu ID. Se a pessoa não existir, retorna 404 Not Found.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Transações retornadas com sucesso", typeof(IEnumerable<TransactionDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Pessoa não encontrada")]
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

    /// <summary>
    /// Retorna uma transação específica pelo seu ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Retorna uma transação específica pelo seu ID",
        Description =
            "Retorna uma transação específica pelo seu ID. Se a transação não existir, retorna 404 Not Found.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Transação retornada com sucesso", typeof(TransactionDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Transação não encontrada")]
    public async Task<ActionResult<TransactionDto>> GetById(int id)
    {
        var transaction = await transactionService.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }

    /// <summary>
    /// Cria uma nova transação.
    /// </summary>
    /// <param name="transactionDto"></param>
    /// <returns></returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cria uma nova transação",
        Description =
            "Cria uma nova transação associada a uma pessoa existente. Se a pessoa não existir, retorna 404 Not Found.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Transação criada com sucesso", typeof(TransactionDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Pessoa não encontrada")]
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

    /// <summary>
    /// Atualiza uma transação existente.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="transactionDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualiza uma transação existente",
        Description =
            "Atualiza uma transação existente identificada pelo seu ID. Se a transação não existir, retorna 404 Not Found.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Transação atualizada com sucesso")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Transação não encontrada")]
    public async Task<ActionResult<TransactionDto>> Update(int id, TransactionDto transactionDto)
    {
        var updated = await transactionService.UpdateAsync(id, transactionDto);
        if (updated == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Exclui uma transação existente pelo seu ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Exclui uma transação existente pelo seu ID",
        Description =
            "Exclui uma transação existente identificada pelo seu ID. Se a transação não existir, retorna 404 Not Found.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Transação excluída com sucesso")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Transação não encontrada")]
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

    /// <summary>
    /// Exclui todas as transações associadas a uma pessoa específica pelo ID da pessoa.
    /// </summary>
    /// <param name="peopleId"></param>
    /// <returns></returns>
    [HttpDelete($"{ApiRoutes.Transaction.GetByPeopleId}/{{peopleId}}")]
    [SwaggerOperation(
        Summary = "Exclui todas as transações associadas a uma pessoa específica pelo ID da pessoa",
        Description =
            "Exclui todas as transações associadas a uma pessoa específica identificada pelo seu ID. Se a pessoa não existir, retorna 404 Not Found.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Transações excluídas com sucesso")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Pessoa não encontrada")]
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