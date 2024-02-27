
using App.Domain.Entities.Process;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistence.Configurations.Process
{
    public class EmployeesConfigurations : IEntityTypeConfiguration<InvEmployees>
    {
        public EmployeesConfigurations()
        {
            //this.HasOptional(emp => emp.ApplicationUser)
            //    .WithOptionalPrincipal(user => user.Employee);
        }
        public void Configure(EntityTypeBuilder<InvEmployees> builder)
        {
            builder.ToTable("InvEmployees");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).IsRequired();
            //builder.HasOne(e => e.branch)
            //       .WithMany(a => a.employees)
            //       .HasForeignKey(p => p.branch_Id).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Job)
                  .WithMany(a => a.Employees)
                  .HasForeignKey(p => p.JobId).OnDelete(DeleteBehavior.NoAction);
            builder.HasIndex(a => a.Code).IsUnique();
            builder.Ignore(e => e.Image);
           
        }
    }
}
