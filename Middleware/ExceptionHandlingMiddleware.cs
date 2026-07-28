using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl.Api.Middleware;

/// <summary>
/// Middleware responsável por capturar exceções não tratadas no pipeline HTTP,
/// registrar/logar o erro e retornar uma resposta JSON padronizada usando <see cref="ProblemDetails"/>.
/// </summary>
/// <remarks>
/// Comportamento:
/// - Registra a exceção usando o <see cref="ILogger{TCategoryName}"/> provido via injeção.
/// - Traduz exceções conhecidas para códigos HTTP apropriados:
///   - <see cref="InvalidOperationException"/> => 400 Bad Request (título: "Invalid operation").
///   - <see cref="ArgumentNullException"/> => 400 Bad Request (título: "Missing required data").
///   - Qualquer outra exceção => 500 Internal Server Error com mensagem genérica ao cliente.
/// - Retorna um payload JSON com <see cref="ProblemDetails"/> contendo Status, Title e Detail.
/// </remarks>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    /// <summary>
    /// Ponto de entrada do middleware que processa a requisição HTTP.
    /// </summary>
    /// <param name="context">Contexto HTTP da requisição atual.</param>
    /// <returns>Uma <see cref="Task"/> que representa a execução assíncrona do middleware.</returns>
    /// <remarks>
    /// - Encapsula a execução do próximo middleware em um bloco try/catch para capturar exceções não tratadas.
    /// - Em caso de exceção:
    ///   1. Registra o erro usando o <c>logger</c> (nível Error).
    ///   2. Mapeia a exceção para um código HTTP, título e detalhe (conforme regras acima).
    ///   3. Cria um objeto <see cref="ProblemDetails"/> com os valores mapeados.
    ///   4. Retorna o <see cref="ProblemDetails"/> como JSON com Content-Type "application/json".
    /// </remarks>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Log the exception
            logger.LogError(ex, "An unhandled exception occurred while processing the request.");

            var (statusCode, title, detail) = ex switch
            {
                InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid operation", ex.Message),
                ArgumentNullException => (HttpStatusCode.BadRequest, "Missing required data", ex.Message),
                _ => (HttpStatusCode.InternalServerError, "An error occurred while processing your request.",
                    "Please try again later or contact support if the issue persists.")
            };

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail
            };

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}