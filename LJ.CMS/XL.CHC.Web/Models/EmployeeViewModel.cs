using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Web.Models
{
    public class EmployeeMainPageViewModel
    {
        public string KeyWord { get; set; }
        public EmployeeBaseInfo EmployeeBaseInfo { get; set; }
        public EmployeeAttachementViewModel AttachementViewModel { get; set; } = new EmployeeAttachementViewModel();

    }

    public class EmployeeAttachementViewModel
    {
        public Nullable<DateTime> MinSearchTime { get; set; }
        public Nullable<DateTime> MaxSearchTime { get; set; }
        public string IDCard { get; set; }

        public IList<CompanyOrder> CompanyOrders { get; set; } = new List<CompanyOrder>();
       
        public EmployeeAttachementExportTarget ExportTarget { get; set; }
        public Guid? ExportOrderId { get; set; }
    }

    public enum EmployeeAttachementExportTarget
    {
        NotifyReport=0,//职业病危害因素告知书.doc
        HealthRegisterForm, //上海市职业健康检查应检者登记表.xls
        HealthResult, //职业卫生监护档案导出.xls
    }
}
