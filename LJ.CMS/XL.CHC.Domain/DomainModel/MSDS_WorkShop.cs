using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class MSDS_WorkShop
    {
        public Guid Id { get; set; }
        public string WorkShop_Name { get; set; }
        public virtual Company Company { get; set; }
        public virtual IList<MSDS_WorkStation> WorkStaions { get; set; }
    }

    public class WorkShopSearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string KeyWord { get; set; }
    }
}
