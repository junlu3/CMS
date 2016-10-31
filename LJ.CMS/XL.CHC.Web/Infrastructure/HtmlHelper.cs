using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using XL.CHC.Domain.Constants;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.UnitOfWork;

namespace XL.CHC.Web.Infrastructure
{
    public static class HtmlExtention
    {
        public static List<MenuItemWithChildren> GetMenuItems()
        {
            IWorkContext workContext = IoCManager.Container.GetInstance<IWorkContext>();
            return workContext.PageMenuItems;
        }

         public static string GetBirthDayByIDCard(this HtmlHelper helper, string idCard)
        {
            return string.Empty;
        }
    }
}
