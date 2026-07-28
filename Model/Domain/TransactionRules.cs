using ExpenseControl.Api.Model.Enums;

namespace ExpenseControl.Api.Model.Domain;

/// <summary>
/// Regras de domínio relacionadas a <see cref="Transaction"/>.
/// </summary>
/// <remarks>
/// Classe estática que agrupa validações e regras de negócio reutilizáveis para transações.
/// Mantém métodos puros (sem efeitos colaterais) que podem ser usados por serviços antes da
/// persistência para garantir conformidade com as regras de negócio da aplicação.
/// </remarks>
public static class TransactionRules
{
    /// <summary>
    /// Determina se é permitido criar uma transação do tipo "Receive" (receita) para uma pessoa com a idade fornecida.
    /// </summary>
    /// <param name="peopleAge">Idade (em anos completos) da pessoa associada à transação.</param>
    /// <param name="type">Tipo da transação (<see cref="TypeTransaction"/>).</param>
    /// <returns>
    /// <c>true</c> se a criação da transação for permitida; caso contrário, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Regras aplicadas:
    /// - Se o tipo não for <see cref="TypeTransaction.Receive"/>, a criação é permitida independentemente da idade.
    /// - Se o tipo for <see cref="TypeTransaction.Receive"/>, a criação é permitida apenas quando <paramref name="peopleAge"/> >= 18.
    /// </remarks>
    /// <example>
    /// Exemplo de uso:
    /// <code>
    /// bool allowed1 = TransactionRules.CanCreateReceiveTransaction(20, TypeTransaction.Receive); // true
    /// bool allowed2 = TransactionRules.CanCreateReceiveTransaction(16, TypeTransaction.Receive); // false
    /// bool allowed3 = TransactionRules.CanCreateReceiveTransaction(15, TypeTransaction.Expense); // true
    /// </code>
    /// </example>
    public static bool CanCreateReceiveTransaction(int peopleAge, TypeTransaction type)
    {
        if (peopleAge < 0)
        {
            throw new ArgumentException("Age cannot be negative.", nameof(peopleAge));
        }

        if (type != TypeTransaction.Receive)
        {
            return true;
        }

        return peopleAge >= 18;
    }
}