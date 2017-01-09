using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.DomainModel;

using XL.CHC.Data.Mapping;
//using XL.CHC.Data.Caching;

namespace XL.CHC.Data.Context
{
    //[DbConfigurationType(typeof(CachingConfiguration))]
    public class CHCContext : DbContext, ICHCContext
    {
        public DbSet<MembershipUser> MembershipUser { get; set; }
        public DbSet<MembershipRole> MembershipRole { get; set; }
        public DbSet <MenuItem> MenuItem { get; set; }
        public DbSet <Category> Category { get; set; }
        public DbSet <CategoryType> CategoryType { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet <CompanyEmployee> CompanyEmployee { get; set; }
        public DbSet <CompanyOrder> CompanyOrder { get; set; }
        public DbSet <CompanySubOrder> CompanySubOrder { get; set; }
        public DbSet<EmployeeBaseInfo> EmployeeBaseInfo { get; set; }
        public DbSet <EmployeeWorkHistory> EmployeeWorkHistory { get; set; }
        public DbSet <HospitalCalendar> HospitoalCalendar { get; set; }
        public DbSet<AutoTask> AutoTask { get; set; }
        public DbSet <HealthResult> HealthResult { get; set; }
        public DbSet<Email> Email { get; set; }
        public DbSet<Law> Laws { get; set; }
        public DbSet<MSDS_Specification> MSDS_Specification { get; set; }
        public DbSet<MSDS_HazardousSubstances> MSDS_HazardousSubstances { get; set;}
        public DbSet<MSDS_H_Statement> MSDS_H_Statement { get; set; }
        public DbSet<MSDS_P_Statement> MSDS_P_Statement { get; set; }
        public DbSet<MSDS_SpecificationCheck> MSDS_SpecificationCheck { get; set; }
        public DbSet<MSDS_Composition> MSDS_Composition { get; set; }
        public DbSet<MSDS_WorkShop> MSDS_WorkShop { get; set; }
        public DbSet<MSDS_WorkStation> MSDS_WorkStation { get; set; }
        public DbSet<MSDS_Worker> MSDS_Worker { get; set; }
        public DbSet<MSDS_Substance_ExposureLimit> MSDS_Substance_ExposureLimit { get; set; }
        public DbSet<MSDS_Customer> MSDS_Customer { get; set; }
        public CHCContext()
        {
            Configuration.LazyLoadingEnabled = true;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Conventions.Add<PluralizingEntitySetNameConvention>();
            modelBuilder.Configurations.Add(new MembershipUserMapping());
            modelBuilder.Configurations.Add(new MembershipRoleMapping());
            modelBuilder.Configurations.Add(new MenuItemMapping());

            modelBuilder.Configurations.Add(new CategoryMapping());
            modelBuilder.Configurations.Add(new CategoryTypeMapping());
            modelBuilder.Configurations.Add(new CompanyEmployeeMapping());
            modelBuilder.Configurations.Add(new CompanyOrderMapping());
            modelBuilder.Configurations.Add(new CompanySubOrderMapping());
            modelBuilder.Configurations.Add(new CompanyMapping());
            modelBuilder.Configurations.Add(new EmployeeBaseInfoMapping());
            modelBuilder.Configurations.Add(new EmployeeWorkHistoryMapping());
            modelBuilder.Configurations.Add(new HospitalCalendarMapping());
            modelBuilder.Configurations.Add(new AutoTaskMapping());
            modelBuilder.Configurations.Add(new HealthResultMapping());
            modelBuilder.Configurations.Add(new EmailMapping());
            modelBuilder.Configurations.Add(new LawMapping());
            modelBuilder.Configurations.Add(new MSDS_SpecificationMapping());
            modelBuilder.Configurations.Add(new MSDS_HazardousSubstancesMapping());
            modelBuilder.Configurations.Add(new MSDS_WorkShopMapping());
            modelBuilder.Configurations.Add(new MSDS_WorkStationMapping());
            base.OnModelCreating(modelBuilder);

        }
        public new void Dispose()
        {

        }
    }
}
