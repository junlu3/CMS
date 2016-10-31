
using StructureMap;
using StructureMap.Web;
using System.Configuration;
using System.Web;
using XL.CHC.Data.Context;
using XL.CHC.Data.Repositories;
using XL.CHC.Data.UnitOfWork;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Domain.Interfaces.UnitOfWork;
using XL.CHC.Services;

namespace XL.CHC.Web
{
    public class IoCRegistry : Registry
    {
        public IoCRegistry()
        {
            //contexts
            For(typeof(IWorkContext)).LifecycleIs(WebLifecycles.HttpSession).Use(typeof(WorkContext));
            For(typeof(IWebHelper)).Use(typeof(WebHelper));
            For(typeof(HttpContextBase)).LifecycleIs(WebLifecycles.HttpSession).Use(new HttpContextWrapper(HttpContext.Current) as HttpContextBase);

            //repositories
            For(typeof(IUnitOfWorkManager)).LifecycleIs(WebLifecycles.HttpContext).Use(typeof(UnitOfWorkManager));
            For(typeof(ICHCContext)).LifecycleIs(WebLifecycles.HttpContext).Use(typeof(CHCContext));


            //services
            For(typeof(IAuthenticationService)).LifecycleIs(WebLifecycles.HttpSession).Use(typeof(FormsAuthenticationService));
            For(typeof(IMembershipService)).Use(typeof(MembershipService));
            For(typeof(IHospitalCalendarService)).Use(typeof(HospitalCalendarService));
            For(typeof(ICompanySubOrderService)).Use(typeof(CompanySubOrderService));
            For(typeof(ICategoryService)).Use(typeof(CategoryService));
            For(typeof(ICompanyService)).Use(typeof(CompanyService));
            For(typeof(ICompanyEmployeeService)).Use(typeof(CompanyEmployeeService));
            For(typeof(IImportExportService)).Use(typeof(ImportExportService));
            For(typeof(IEmployeeWorkHistoryService)).Use(typeof(EmployeeWorkHistoryService));
            For(typeof(ICompanyOrderService)).Use(typeof(CompanyOrderService));
            For(typeof(IHealthResultService)).Use(typeof(HealthResultService));
            For(typeof(IMenuItemService)).Use(typeof(MenuItemService));
            For(typeof(IEmployeeBaseInfoService)).Use(typeof(EmployeeBaseInfoService));
            For(typeof(ILawService)).Use(typeof(LawService));
            For(typeof(IEmailService)).Use(typeof(EmailService));

            For(typeof(IMSDS_SpecificationService)).Use(typeof(MSDS_SpecificationService));
            For(typeof(IMSDS_HazardousSubstancesService)).Use(typeof(MSDS_HazardousSubstancesService));
            For(typeof(IMSDS_H_StatementService)).Use(typeof(MSDS_H_StatementService));
            For(typeof(IMSDS_P_StatementService)).Use(typeof(MSDS_P_StatementService));
            For(typeof(IMSDS_SpecificationCheckService)).Use(typeof(MSDS_SpecificationCheckService));
            For(typeof(IMSDS_CompositionService)).Use(typeof(MSDS_CompositionService));
            For(typeof(IMSDS_WorkShopService)).Use(typeof(MSDS_WorkShopService));
            For(typeof(IMSDS_WorkStationService)).Use(typeof(MSDS_WorkStationService));
            For(typeof(IMSDS_WorkerService)).Use(typeof(MSDS_WorkerService));
            For(typeof(IMSDS_Substance_ExposureLimitService)).Use(typeof(MSDS_Substance_ExposureLimitService));


            //repository
            For(typeof(IEmailRepository)).Use(typeof(EmailRepository));
            For(typeof(IEmployeeBaseInfoRepository)).Use(typeof(EmployeeBaseInfoRepository));
            For(typeof(IMenuItemRepository)).Use(typeof(MenuItemRepository));
            For(typeof(ICompanyOrderRepository)).Use(typeof(CompanyOrderRepository));
            For(typeof(ICompanyRepository)).Use(typeof(CompanyRepository));
            For(typeof(ICategoryRepository)).Use(typeof(CategoryRepository));
            For(typeof(ICompanySubOrderRepository)).Use(typeof(CompanySubOrderRepository));
            For(typeof(IMembershipRepository)).Use(typeof(MembershipRepository));
            For(typeof(ICompanyEmployeeRepository)).Use(typeof(CompanyEmployeeRepository));
            For(typeof(IEmployeeBaseInfoRepository)).Use(typeof(EmployeeBaseInfoRepository));
            For(typeof(IEmployeeWorkHistoryRepository)).Use(typeof(EmployeeWorkHistoryRepository));
            For(typeof(IHealthResultRepository)).Use(typeof(HealthResultRepository));
            For(typeof(IHospitalCalendarRepository)).Use(typeof(HospitalCalendarRepository));
            For(typeof(ILawRepository)).Use(typeof(LawRepository));

            For(typeof(IMSDS_SpecificationRepository)).Use(typeof(MSDS_SpecificationRepository));
            For(typeof(IMSDS_HazardousSubstancesRepository)).Use(typeof(MSDS_HazardousSubstancesRepository));
            For(typeof(IMSDS_H_StatementRepository)).Use(typeof(MSDS_H_StatementRepository));
            For(typeof(IMSDS_P_StatementRepository)).Use(typeof(MSDS_P_StatementRepository));
            For(typeof(IMSDS_SpecificationCheckRepository)).Use(typeof(MSDS_SpecificationCheckRepository));
            For(typeof(IMSDS_CompositionRepository)).Use(typeof(MSDS_CompositionRepository));
            For(typeof(IMSDS_WorkShopRepository)).Use(typeof(MSDS_WorkShopRepository));
            For(typeof(IMSDS_WorkStationRepository)).Use(typeof(MSDS_WorkStationRepository));
            For(typeof(IMSDS_WorkerRepository)).Use(typeof(MSDS_WorkerRepository));
            For(typeof(IMSDS_Substance_ExposureLimitRepository)).Use(typeof(MSDS_Substance_ExposureLimitRepository));
        }
    }
}
