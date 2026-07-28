namespace ExpenseControl.Api.Controllers;

public static class ApiRoutes
{
    public const string Root = "api";

    public static class Auth
    {
        public const string Base = Root + "/auth";
        public const string Login = "login";
        public const string Register = "register";
        public const string Users = "users";
    }

    public static class People
    {
        public const string Base = Root + "/people";
    }

    public static class Transaction
    {
        public const string Base = Root + "/transaction";
        public const string GetByPeopleId = "people";
        public const string Totals = "totals";
    }
}