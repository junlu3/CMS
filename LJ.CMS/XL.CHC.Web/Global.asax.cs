using DataAnnotationsExtensions.ClientValidation;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using XL.CHC.Domain.Interfaces.UnitOfWork;
using XL.CHC.Web.Infrastructure;

namespace XL.CHC.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public IUnitOfWorkManager UnitOfWorkManager
        {
            get { return IoCManager.Container.GetInstance<IUnitOfWorkManager>(); }
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            IoCManager.Configure();
            ControllerBuilder.Current.SetControllerFactory(new IoCControllerFactory());
            // Register Data annotations
            DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();
        }

        private void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CHCAuthorizeAttribute());
            filters.Add(new CHCExceptionAttribute());
        }
    }
}
