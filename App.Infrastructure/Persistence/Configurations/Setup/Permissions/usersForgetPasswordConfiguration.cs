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
    public class usersForgetPasswordConfiguration : IEntityTypeConfiguration<usersForgetPassword>
    {
        public void Configure(EntityTypeBuilder<usersForgetPassword> builder)
        {
            builder.ToTable("usersForgetPassword");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.userAccount).WithMany(x => x.usersForgetPasswords).HasForeignKey(x => x.userId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
