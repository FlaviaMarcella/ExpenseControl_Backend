using ExpenseControl.Api.Model.Enums;

namespace ExpenseControl.Api.Model.Domain;

public static class TransactionRules
{
    public static bool CanCreateReceiveTransaction(int peopleAge, TypeTransaction type)
    {
        if (type != TypeTransaction.Receive)
        {
            return true;
        }

        return peopleAge >= 18;
    }
}