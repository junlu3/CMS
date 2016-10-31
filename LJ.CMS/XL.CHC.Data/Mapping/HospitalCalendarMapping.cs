
using System.Data.Entity.ModelConfiguration;

using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class HospitalCalendarMapping : EntityTypeConfiguration<HospitalCalendar>
    {
        public HospitalCalendarMapping()
        {
            HasKey(x => x.Id);
        }
    }
}
