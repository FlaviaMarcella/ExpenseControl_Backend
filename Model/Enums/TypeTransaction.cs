using System.ComponentModel;
using System.Reflection;

namespace ExpenseControl.Api.Model.Enums;

public enum TypeTransaction
{
    [Description("Receita")] Receive,
    [Description("Despesa")] Expense
}

public static class TypeTransactionExtensions
{
    public static string getDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString()) ?? throw new InvalidOperationException();
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}