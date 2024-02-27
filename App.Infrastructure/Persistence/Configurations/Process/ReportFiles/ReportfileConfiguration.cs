using App.Domain;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace App.Infrastructure
{
    public class ReportMangerConfiguration : IEntityTypeConfiguration<ReportManger>
    {
        public void Configure(EntityTypeBuilder<ReportManger> builder)
        {
            builder.ToTable("ReportManger");
            builder.HasKey(e => e.Id);

            builder.HasOne(h => h.Files)
                .WithMany(h => h.reportmanger)
                .HasForeignKey(fileManger => fileManger.ArabicFilenameId).OnDelete(DeleteBehavior.NoAction);
            //.HasForeignKey(h=> h.ara);

            builder.HasOne(h => h.ScreenNames)
    .WithMany(fileManger => fileManger.FileManger)
    .HasForeignKey(fileManger => fileManger.screenId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
