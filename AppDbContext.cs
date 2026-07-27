using Microsoft.EntityFrameworkCore;

namespace ExpenseControl.Api;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<People> Peoples => Set<People>();
}