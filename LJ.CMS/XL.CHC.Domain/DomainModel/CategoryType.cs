using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class CategoryType
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string TypeName { get; set; }

        public virtual IList<Category> Categories { get; set; }
    }
}
