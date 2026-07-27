using System.ComponentModel;
using System.Reflection;

namespace ExpenseControl.Api.Model.Enums;

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

public static class RelationshipExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString()) ?? throw new InvalidOperationException();
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}