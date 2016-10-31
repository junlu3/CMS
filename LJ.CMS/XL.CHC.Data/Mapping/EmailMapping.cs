using System.Data.Entity.ModelConfiguration;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class EmailMapping : EntityTypeConfiguration<Email>
    {
        public EmailMapping()
        {
            this.ToTable("Email");
            this.HasKey(e => e.Id);

            this.Property(e => e.To).IsRequired().HasMaxLength(500);
            this.Property(e => e.ToName).HasMaxLength(500);
            this.Property(e => e.ReplyTo).HasMaxLength(500);
            this.Property(e => e.ReplyToName).HasMaxLength(500);
            this.Property(e => e.CC).HasMaxLength(500);
            this.Property(e => e.Bcc).HasMaxLength(500);
            this.Property(e => e.Subject).HasMaxLength(500);
            this.Property(e => e.CreatedBy).HasMaxLength(1000);
        }
    }
}
