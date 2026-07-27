using ExpenseControl.Api.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseControl.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<People> Peoples => Set<People>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
}