using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Constants
{
    public class MenuItemWithChildren
    {
        public MenuItem MenuItem { get; set; } = new MenuItem();
        public List<MenuItem> SubMenuItems { get; set; } = new List<MenuItem>();
    }
}
