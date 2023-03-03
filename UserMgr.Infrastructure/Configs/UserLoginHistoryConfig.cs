using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserMgr.Domain.Entities;

namespace UserMgr.Infrastructure.Configs;

public class UserLoginHistoryConfig: IEntityTypeConfiguration<UserLoginHistory>
{
    public void Configure(EntityTypeBuilder<UserLoginHistory> builder)
    {
        builder.ToTable("t_userLoginHistory");
        builder.OwnsOne(e => e.PhoneNumber, p =>
        {
            p.Property(p => p.RegionNumber).HasMaxLength(5).IsUnicode(false);
            p.Property(p => p.Number).HasMaxLength(20).IsUnicode(false);
        });
    }
}