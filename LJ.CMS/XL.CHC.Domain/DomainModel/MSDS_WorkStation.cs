using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class MSDS_WorkStation
    {
        public Guid Id { get; set; }
        public string WorkStation_Name { get; set; }
        public virtual MSDS_WorkShop WorkShop { get; set; }
        public virtual IList<MSDS_Worker> Workers { get; set; }
        public virtual IList<MSDS_Specification> Specifications { get; set; }
    }

    public class WorkStationSearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string KeyWord { get; set; }
        public Guid WorkShop_Id { get; set; }
    }
}
