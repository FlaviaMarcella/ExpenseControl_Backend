namespace ExpenseControl.Api.Controllers;

/// <summary>
/// Centraliza os templates de rota da API, evitando strings "mágicas" espalhadas
/// pelos atributos <c>[Route]</c>/<c>[HttpGet]</c> de cada Controller.
/// </summary>
public static class ApiRoutes
{
    /// <summary>Prefixo raiz de toda a API.</summary>
    public const string Root = "api";

    /// <summary>Rotas do <see cref="AuthController"/>.</summary>
    public static class Auth
    {
        public const string Base = Root + "/auth";
        public const string Login = "login";
        public const string Register = "register";
    }

    /// <summary>Rotas do <c>PeopleController</c>.</summary>
    public static class People
    {
        public const string Base = Root + "/people";
    }

    /// <summary>Rotas do <c>TransactionController</c>.</summary>
    public static class Transaction
    {
        public const string Base = Root + "/transaction";

        /// <summary>Segmento de sub-rota para endpoints filtrados por pessoa (ex.: <c>api/transaction/people/{peopleId}</c>).</summary>
        public const string GetByPeopleId = "/people";
    }
}