using System.Web.Mvc;

namespace XL.CHC.Web
{
    public class IoCControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, System.Type controllerType)
        {
            if (controllerType != null)
            {
                Controller c = IoCManager.Container.GetInstance(controllerType) as Controller;
                return c;
            }
            else
            {
                return null;
            }
        }
    }
}
