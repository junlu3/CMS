using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class MSDS_H_Statement
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public virtual List<MSDS_HazardousSubstances> HazardousSubstances { get; set; }
    }
}
