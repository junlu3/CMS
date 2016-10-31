using System.Data.Entity.ModelConfiguration;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class AutoTaskMapping : EntityTypeConfiguration<AutoTask>
    {
        public AutoTaskMapping()
        {
            this.ToTable("AutoTask");
            this.HasKey(x => x.Id);

            this.Property(x => x.Name).IsRequired().HasMaxLength(50);
            this.Property(x => x.Code).IsRequired().HasMaxLength(50);
            this.Property(x => x.Comment).HasMaxLength(200);

            this.HasOptional(x => x.AutoTaskStatus)
                .WithMany()
                .Map(x => x.MapKey("AutoTaskStatus"));

            this.HasOptional(x => x.AutoTaskType)
                .WithMany()
                .Map(x => x.MapKey("AutoTaskType"));
        }
    }
}
