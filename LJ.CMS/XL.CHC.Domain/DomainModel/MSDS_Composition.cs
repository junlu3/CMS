using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class MSDS_Composition
    {
        public Guid Id { get; set; }
        public string Composition_Name { get; set; }
        public string CASCode { get; set; }
    }
}
