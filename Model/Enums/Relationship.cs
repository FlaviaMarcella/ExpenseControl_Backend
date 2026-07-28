using System.ComponentModel;
using System.Reflection;

namespace ExpenseControl.Api.Model.Enums;

/// <summary>
/// Grau de parentesco de uma <see cref="Entity.People"/> em relação ao núcleo familiar.
/// </summary>
/// <remarks>
/// Serializado como texto (string) nas requisições/respostas JSON da API — configurado via
/// <c>JsonStringEnumConverter</c> no <c>Program.cs</c> — em vez do índice numérico padrão do enum.
/// Cada valor carrega um <see cref="System.ComponentModel.DescriptionAttribute"/> com a tradução
/// em português, acessível via <see cref="RelationshipExtensions.GetDescription"/>.
/// </remarks>
public enum Relationship
{
    [Description("Pai")] Father,
    [Description("Mãe")] Mother,
    [Description("Filho(a)")] Child,
    [Description("Irmão(ã)")] Sibling,
    [Description("Avô(ó)")] Grandparent,
    [Description("Tio(a)")] UncleAunt,
    [Description("Primo(a)")] Cousin,
    [Description("Amigo(a)")] Friend,
    [Description("Outro")] Other
}

/// <summary>Métodos de extensão para exibição amigável de valores de <see cref="Enum"/>.</summary>
public static class RelationshipExtensions
{
    /// <summary>Retorna o texto do <see cref="System.ComponentModel.DescriptionAttribute"/> do valor, ou o nome do enum caso não haja um.</summary>
    public static string GetDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString()) ?? throw new InvalidOperationException();
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}