using App.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process
{
    public class GLJournalEntryDraftDetailsConfigurations : IEntityTypeConfiguration<GLJournalEntryDraftDetails>
    {
        public void Configure(EntityTypeBuilder<GLJournalEntryDraftDetails> builder)
        {
            builder.ToTable("GLJournalEntryDraftDetails");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.DescriptionAr).HasColumnName("DescriptionAr");
            builder.Property(e => e.DescriptionEn).HasColumnName("DescriptionEn");
            builder.HasOne(e => e.JournalEntryDraft)
                   .WithMany(d => d.journalEntryDraftDetails)
                   .HasForeignKey(q => q.JournalEntryDraftId);

        }
    }
}
