using System.ComponentModel;
using System.Reflection;

namespace ExpenseControl.Api.Model.Enums;

/// <summary>
/// Classifica um <see cref="Entity.Transaction"/> como entrada (receita) ou saída (despesa) de dinheiro.
/// </summary>
/// <remarks>
/// Serializado como texto (string) no JSON da API, não como número — ver configuração
/// de <c>JsonStringEnumConverter</c> em <c>Program.cs</c>. A regra de negócio associada
/// (menores de idade não podem ter transações do tipo <see cref="Receive"/>) fica em
/// <see cref="Domain.TransactionRules.CanCreateReceiveTransaction"/>.
/// </remarks>
public enum TypeTransaction
{
    /// <summary>Entrada de dinheiro (receita).</summary>
    [Description("Receita")] Receive,

    /// <summary>Saída de dinheiro (despesa).</summary>
    [Description("Despesa")] Expense
}

/// <summary>Métodos de extensão para exibição amigável de valores de <see cref="TypeTransaction"/>.</summary>
public static class TypeTransactionExtensions
{
    /// <summary>Retorna o texto do <see cref="DescriptionAttribute"/> do valor, ou o nome do enum caso não haja um.</summary>
    public static string getDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString()) ?? throw new InvalidOperationException();
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}