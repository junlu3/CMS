using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Domain.Interfaces.UnitOfWork;
using XL.CHC.Web.Infrastructure;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Controllers
{
    [CHCAuthorize]
    public class BaseController : Controller
    {
        protected IUnitOfWorkManager UnitOfWorkManager { get; set; }
        protected IMembershipService MembershipService { get; set; } 
        protected IWorkContext WorkContext { get; set; }

      

        public BaseController( )
        {
            UnitOfWorkManager = IoCManager.Container.GetInstance<IUnitOfWorkManager>(); ;
            MembershipService = IoCManager.Container.GetInstance<IMembershipService>(); ;
            WorkContext = IoCManager.Container.GetInstance<IWorkContext>();
            //var authenticationService = DependencyResolver.Current.GetService<IAuthenticationService>();
            //var user = MembershipService.GetUser("admin");
            //authenticationService.SignIn(user, true);

            //WorkContext.CurrentMembershipUser = user;
        }

        protected virtual void SuccessNotification(string message, bool persistForNextRequest = true)
        {
            AddNotification(NotifyType.Success, message, persistForNextRequest);
        }

        protected virtual void ErrorNotification(Exception ex, bool persistForNextRequest = true)
        {
            AddNotification(NotifyType.Error, ex.Message, persistForNextRequest);
        }

        private void AddNotification(NotifyType notifyType, string message, bool persistForTheNextRequest)
        {
            string dataKey = string.Format("XL.CHC.notifications.{0}", notifyType);
            if (persistForTheNextRequest)
            {
                if (TempData[dataKey] == null)
                    TempData[dataKey] = new List<string>();
                ((List<string>)TempData[dataKey]).Add(message);
            }
            else
            {
                if (ViewData[dataKey] == null)
                    ViewData[dataKey] = new List<string>();
                ((List<string>)ViewData[dataKey]).Add(message);
            }
        }
    }
}
