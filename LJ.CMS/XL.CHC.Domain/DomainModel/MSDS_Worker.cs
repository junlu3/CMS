using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class MSDS_Worker
    {
        public Guid Id { get; set; }
        [MaxLength(50, ErrorMessage = "名称长度不大于50位")]
        public string Worker_Name {get;set;}
        [MaxLength(20, ErrorMessage = "名称长度不大于20位")]
        public string Worker_ID { get; set; }
        public virtual IList<MSDS_WorkStation> WorkStations { get; set; } = new List<MSDS_WorkStation>();
        public virtual Company Company { get; set; }
    }

    public class WorkerSearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string KeyWord { get; set; }
        public string WorkStation_Id { get; set; }
    }
}
