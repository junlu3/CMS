using System.Data.Entity.ModelConfiguration;

using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Data.Mapping
{
    public class EmployeeBaseInfoMapping : EntityTypeConfiguration<EmployeeBaseInfo>
    {
        public EmployeeBaseInfoMapping()
        {
            HasKey(x => x.Id);
            HasMany(x => x.CompanyEmployees).WithRequired(x => x.EmployeeBaseInfo)
               .Map(x => x.MapKey("EmployeeBaseInfo_Id"));
            HasMany(x => x.WorkHistories).WithRequired(x => x.EmployeeBaseInfo)
                .Map(x => x.MapKey("EmployeeBaseInfo_Id"));
            
        }
    }
}
