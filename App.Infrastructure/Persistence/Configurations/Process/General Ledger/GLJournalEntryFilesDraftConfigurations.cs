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
    public class GLJournalEntryFilesDraftConfigurations : IEntityTypeConfiguration<GLJournalEntryFilesDraft>
    {
        public void Configure(EntityTypeBuilder<GLJournalEntryFilesDraft> builder)
        {
            builder.ToTable("GLJournalEntryFilesDraft");
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.journalEntryDraft)
                   .WithMany(d => d.journalEntryFilesDrafts)
                   .HasForeignKey(q => q.JournalEntryDraftId);

        }
    }
}
