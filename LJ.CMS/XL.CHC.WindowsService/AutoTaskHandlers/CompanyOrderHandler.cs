using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Services;
using XL.Utilities;

namespace XL.CHC.WindowsService.AutoTaskHandlers
{
    public class CompanyOrderHandler : BaseAutoTaskHandler
    {
        public override void HandleAutoTask()
        {
            LogManager.Instance.Write("Start CompanyOrder Handler ******************************************************");

            using (var cache = new EntityCache<CompanyOrder, CHCContext>(p => p.Locked == true&& p.IsBuildCompleted == false))
            {
                while (true)
                {
                    LogManager.Instance.Write(cache.Results.Count().ToString());

                    var importExportService = new ImportExportService();
                    var companyOrderService = new CompanyOrderService(null, importExportService, null,null);
                    if (cache.Results.Count() > 0)
                    {
                        foreach (var order in cache.Results)
                        {
                            companyOrderService.ExportCompanyOrderForms(ConfigurationManager.AppSettings["CHCWebRootPath"], order);

                            using (var context = new CHCContext())
                            {
                                var entity = context.CompanyOrder.SingleOrDefault(e => e.Id == order.Id);
                                entity.Locked = true;
                                entity.IsBuildCompleted = true;
                                context.SaveChanges();
                            }
                        }
                    }

                    Thread.Sleep(5000);
                }
            }
        }
    }
}
