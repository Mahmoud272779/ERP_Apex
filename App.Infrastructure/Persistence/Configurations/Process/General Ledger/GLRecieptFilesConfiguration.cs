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
    public class GLRecieptFilesConfiguration : IEntityTypeConfiguration<GLRecieptFiles>
    {
        public void Configure(EntityTypeBuilder<GLRecieptFiles> builder)
        {
            builder.ToTable("GLRecieptFiles");
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.reciepts)
                   .WithMany(d => d.RecieptsFiles)
                   .HasForeignKey(q => q.RecieptId);

        }
    }
}
