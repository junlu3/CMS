using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces
{
    public partial interface IAuthenticationService
    {
        void SignIn(MembershipUser membershipUser, bool createPersistentCookie);
        void SignOut();
        MembershipUser GetAuthenticatedMembershipUser();
    }
}
