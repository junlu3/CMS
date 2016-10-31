using System.Data.Entity.ModelConfiguration;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class MSDS_WorkStationMapping : EntityTypeConfiguration<MSDS_WorkStation>
    {
        public MSDS_WorkStationMapping()
        {
            HasKey(x=>x.Id);
            HasMany(x => x.Workers)
                .WithMany(x => x.WorkStations)
                .Map(m => {
                    m.MapLeftKey("WorkerStation_Id");
                    m.MapRightKey("Worker_Id");
                    m.ToTable("MSDS_WorkStation_Worker_Mapping");
                });
        }

    }
}
