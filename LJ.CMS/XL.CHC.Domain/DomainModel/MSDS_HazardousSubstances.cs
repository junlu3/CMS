using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace XL.CHC.Domain.DomainModel
{
    public class MSDS_HazardousSubstances
    {
        [Key]
        public Guid HS_Id { get; set; }
        public string HS_Name { get; set; }
        public string HS_MinPercent { get; set; }
        public string HS_MaxPercent { get; set; }
        public string HS_CASCode { get; set; }
        public virtual IList<MSDS_Specification> Specifications { get; set; }
        public virtual IList<MSDS_H_Statement> HS_HStatements { get; set; } = new List<MSDS_H_Statement>();

    }
}
