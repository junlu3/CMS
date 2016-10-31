using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class LawMapping : EntityTypeConfiguration<Law>
    {
        public LawMapping()
        {
            this.ToTable("Law");
            this.HasKey(e => e.Id);

            this.Property(e => e.Name).IsRequired().HasMaxLength(1000);
            this.Property(e => e.DocumentNumber).HasMaxLength(100);
            this.Property(e => e.FilePath).IsRequired().HasMaxLength(1000);
            this.Property(e => e.CreatedBy).HasMaxLength(1000);
        }
    }
}
