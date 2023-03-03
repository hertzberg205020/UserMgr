using Microsoft.EntityFrameworkCore;
using UserMgr.Domain.Entities;

namespace UserMgr.Infrastructure;

public class UserDbContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserLoginHistory> LoginHistories { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options): base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }
}