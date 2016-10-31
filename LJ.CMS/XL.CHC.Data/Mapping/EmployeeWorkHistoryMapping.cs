
using System.Data.Entity.ModelConfiguration;

using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class EmployeeWorkHistoryMapping : EntityTypeConfiguration<EmployeeWorkHistory>
    {
        public EmployeeWorkHistoryMapping()
        {
            HasKey(x => x.Id);
        }
    }
}
