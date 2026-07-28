using ExpenseControl.Api.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseControl.Api.Data;

/// <summary>
/// Representa o contexto de banco de dados do Entity Framework Core para a API ExpenseControl.
/// </summary>
/// <remarks>
/// Este contexto expõe propriedades <see cref="DbSet{TEntity}"/> para as entidades da aplicação
/// e é configurado via injeção de dependência usando <see cref="DbContextOptions{AppDbContext}"/>.
///— Uso típico: registre este contexto no contêiner de DI (por exemplo, em <c>Program.cs</c>) e deixe o EF Core
/// gerenciar a string de conexão, as migrações e o rastreamento de alterações (change tracking).
/// </remarks>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Inicializa uma nova instância de <see cref="AppDbContext"/> usando as opções especificadas.
    /// </summary>
    /// <param name="options">
    /// As opções a serem usadas pelo <see cref="DbContext"/>. Essas opções normalmente incluem
    /// o provedor (por exemplo, SQLite), a string de conexão e outras configurações do EF Core.
    /// Este construtor destina-se a ser chamado pelo contêiner de injeção de dependência.
    /// </param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Obtém um <see cref="DbSet{TEntity}"/> que representa a tabela de usuários (Users).
    /// </summary>
    /// <remarks>
    /// Utilize esta propriedade para consultar e persistir instâncias de <see cref="User"/>.
    /// </remarks>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Obtém um <see cref="DbSet{TEntity}"/> que representa a tabela de pessoas (People).
    /// </summary>
    /// <remarks>
    /// Utilize esta propriedade para consultar e persistir instâncias de <see cref="People"/>.
    /// A propriedade usa o acessor <see cref="DbContext.Set{TEntity}"/> para garantir integração
    /// correta com o rastreamento de alterações do EF Core e com as migrações.
    /// </remarks>
    public DbSet<People> Peoples => Set<People>();

    /// <summary>
    /// Obtém um <see cref="DbSet{TEntity}"/> que representa a tabela de transações (Transaction).
    /// </summary>
    /// <remarks>
    /// Utilize esta propriedade para consultar e persistir instâncias de <see cref="Transaction"/>.
    /// O nome segue a forma pluralizada para coleções de entidades.
    /// </remarks>
    public DbSet<Transaction> Transactions => Set<Transaction>();
}