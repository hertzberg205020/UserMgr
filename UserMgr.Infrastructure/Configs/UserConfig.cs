using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserMgr.Domain.Entities;

namespace UserMgr.Infrastructure.Configs;

public class UserConfig: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("t_user");
        builder.OwnsOne(e => e.PhoneNumber, x =>
        {
            x.Property(p => p.RegionNumber).HasMaxLength(5).IsUnicode(false);
            x.Property(p => p.Number).HasMaxLength(20).IsUnicode(false);
        });
        builder.Property("_passwordHash").HasMaxLength(255).IsUnicode(false);
        builder.HasOne(x => x.AccessFail)
            .WithOne(x => x.User)
            .HasForeignKey<UserAccessFail>(x => x.UserId);
    }
}