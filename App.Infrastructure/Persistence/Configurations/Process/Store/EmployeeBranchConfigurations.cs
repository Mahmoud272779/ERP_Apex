using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Entities.Process.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistence.Configurations.Process.Store
{
    public class EmployeeBranchConfigurations : IEntityTypeConfiguration<InvEmployeeBranch>
    {
        public void Configure(EntityTypeBuilder<InvEmployeeBranch> builder)
        {
            builder.ToTable("InvEmployeesBranches");
            builder.HasKey(e => new { e.EmployeeId, e.BranchId });
            builder.HasOne(employeeBranch => employeeBranch.Employee)
                .WithMany(employee => employee.EmployeeBranches)
                .HasForeignKey(employeeBranch => employeeBranch.EmployeeId);
            builder.HasOne(employeeBranch => employeeBranch.Branch)
                .WithMany(branch => branch.EmployeeBranches)
                .HasForeignKey(employeeBranch => employeeBranch.BranchId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
