using System;
using System.Linq;
using System.Web.Mvc;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.UnitOfWork;

namespace XL.CHC.Web.Infrastructure
{
    public class CHCAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                   || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
            if (skipAuthorization)
                return;
            IWorkContext workContext = IoCManager.Container.GetInstance<IWorkContext>();
            string controller = filterContext.RouteData.Values["controller"].ToString();
            var currentUser = workContext.CurrentMembershipUser;
            if (currentUser == null)
            {
                filterContext.Result = new RedirectResult("~/MembershipUser/Login?ReturnUrl=" + filterContext.RequestContext.HttpContext.Request.RawUrl);
            }
            else
            {
                if (!IsAllowed(workContext,  controller))
                {
                    throw new Exception("您没有权限访问请求的页面，请联系管理员。");
                }
            }
            //base.OnAuthorization(filterContext);
        }

       
        private bool IsAllowed(IWorkContext workContext, string controller)
        {
            //IUnitOfWorkManager unitOfWorkManager = DependencyResolver.Current.GetService<IUnitOfWorkManager>();
            IUnitOfWorkManager unitOfWorkManager = IoCManager.Container.GetInstance<IUnitOfWorkManager>();
            using (unitOfWorkManager.NewUnitOfWork())
            {
                var user = workContext.CurrentMembershipUser;
                foreach (var role in user.MembershipRoles)
                {
                    if(role.MenuItems.FirstOrDefault(x=>x.Controllor!=null && x.Controllor.ToLower()== controller.ToLower())!=null )
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
