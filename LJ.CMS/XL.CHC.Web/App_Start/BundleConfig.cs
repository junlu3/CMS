using System.Web;
using System.Web.Optimization;

namespace XL.CHC.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/commonCss").Include(
                
                ));
        }
    }
}
