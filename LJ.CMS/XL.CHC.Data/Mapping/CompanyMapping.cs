
using System.Data.Entity.ModelConfiguration;

using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class CompanyMapping : EntityTypeConfiguration<Company>
    {
        public CompanyMapping()
        {
            HasKey(x => x.Id);
            HasOptional(x => x.CompanyType)
                .WithMany()
                .Map(x => x.MapKey("CompanyType"));
            HasOptional(x => x.CompanyRegisterType)
                .WithMany()
                .Map(x => x.MapKey("CompanyRegisterType"));
            HasMany(x => x.CompanyEmployees)
                .WithRequired(x => x.Company)
                .Map(x => x.MapKey("Company_Id"));
            HasMany(x => x.CompanyOrders)
                .WithRequired(x => x.Company)
                .Map(x => x.MapKey("Company_Id"));
            HasMany(x => x.CompanySubOrders).WithRequired(x => x.Company)
               .Map(x => x.MapKey("Company_Id"));
            HasMany(x => x.MembershipUsers)
                .WithRequired(x => x.Company)
                .Map(x=>x.MapKey("Company_Id"));
            HasMany(x => x.Specifications)
                .WithRequired(x => x.Company)
                .Map(x => x.MapKey("Company_Id"));
            HasMany(x => x.MembershipRoles)
                .WithRequired(x => x.Company)
                .Map(x=>x.MapKey("Company_Id"));
            HasMany(x=>x.WorkShops)
                .WithRequired(x=>x.Company)
                .Map(x=>x.MapKey("Company_Id"));
            HasMany(x => x.Workers)
                .WithRequired(x=>x.Company)
                .Map(x=>x.MapKey("Company_Id"));
                
        }
    }
}
