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
    public class userBranchesConfiguration : IEntityTypeConfiguration<userBranches>
    {
        public void Configure(EntityTypeBuilder<userBranches> builder)
        {
            builder.ToTable("userBranches");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.GLBranch).WithMany(x => x.userBranches).HasForeignKey(x => x.GLBranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.userAccount).WithMany(x => x.userBranches).HasForeignKey(x => x.userAccountId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
