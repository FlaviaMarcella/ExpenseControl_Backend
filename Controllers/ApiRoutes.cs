namespace ExpenseControl.Api.Controllers;

public static class ApiRoutes
{
    public const string Root = "api";

    public static class People
    {
        public const string Base = Root + "/people";
    }

    public static class Transaction
    {
        public const string Base = Root + "/transaction";
        public const string GetByPeopleId = Base + "/people";
    }
}