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
    public class GLJournalEntryFilesConfigurations : IEntityTypeConfiguration<GLJournalEntryFiles>
    {
        public void Configure(EntityTypeBuilder<GLJournalEntryFiles> builder)
        {
            builder.ToTable("GLJournalEntryFiles");
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.JournalEntry)
                   .WithMany(d => d.journalEntryFiles)
                   .HasForeignKey(q => q.JournalEntryId);

        }
    }
}
