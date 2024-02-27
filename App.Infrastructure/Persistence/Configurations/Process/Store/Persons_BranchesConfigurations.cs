 using App.Domain.Entities.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process
{
    public class Persons_BranchesConfigurations : IEntityTypeConfiguration<InvPersons_Branches>
    {
        public void Configure(EntityTypeBuilder<InvPersons_Branches> builder)
        {
            builder.ToTable("InvPersons_Branches");
            builder.HasKey(e => new { e.BranchId, e.PersonId });

            builder.HasOne(e => e.Person)
                   .WithMany(a => a.PersonBranch)
                   .HasForeignKey(p => p.PersonId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Branch)
                   .WithMany(a => a.PersonBranch)
                   .HasForeignKey(p => p.BranchId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
