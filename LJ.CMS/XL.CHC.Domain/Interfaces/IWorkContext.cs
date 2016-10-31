using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.Constants;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces
{
    public interface IWorkContext
    {
        MembershipUser CurrentMembershipUser { get; set; }

        List<MenuItemWithChildren> PageMenuItems { get; set; }
        void RemoveMembershipUser();
        void ClearMenuItems();
    }

  
}
