using App.Domain.Entities;
using App.Domain.Entities.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Setup
{
    public class userAccountConfiguration : IEntityTypeConfiguration<userAccount>
    {
        public void Configure(EntityTypeBuilder<userAccount> builder)
        {
            builder.ToTable("userAccount");
            builder.HasKey(x => x.id);
            builder.HasOne(x => x.employees).WithMany(x => x.userAccount).HasForeignKey(x => x.employeesId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.permissionList).WithMany(x => x.userAccount).HasForeignKey(x => x.permissionListId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x=> x.signinLogs).WithOne(x=> x.userAccount).HasForeignKey(x=> x.userAccountid).OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}
