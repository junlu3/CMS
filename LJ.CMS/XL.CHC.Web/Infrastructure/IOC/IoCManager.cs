using StructureMap;

namespace XL.CHC.Web
{
    public class IoCManager
    {
        public static Container Container;

        public static void Configure()
        {
            Container = new Container(x =>
            {
                x.AddRegistry(new IoCRegistry());

                x.Scan(scanner =>
                {
                    scanner.Assembly("XL.CHC.Data");
                    scanner.Assembly("XL.CHC.Domain");
                    scanner.Assembly("XL.CHC.Services");
                });
            });
        }
    }
}
