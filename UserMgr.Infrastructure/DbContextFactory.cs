using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UserMgr.Infrastructure;

public class DbContextFactory: IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<UserDbContext> builder = new();
        string? connStr = Environment.GetEnvironmentVariable("ConnectionStrings:Demo3");
        builder.UseSqlServer(connStr);
        return new UserDbContext(builder.Options);
    }
}