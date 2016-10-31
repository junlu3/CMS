using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IMembershipService
    {
        IList<MembershipUser> GetAllUsers();
        IList<MembershipUser> GetAllUsersByComapany(Guid company_Id);
        MembershipUser GetUser(string userName);
        MembershipUser GetUser(Guid id);
        void AddUser(MembershipUser user);
        void DeleteUser(Guid id);

        MembershipRole GetRole(Guid id);
        IList<MembershipRole> GetAllRoles();
        IList<MembershipRole> GetAllRolesByCompany(Guid id);
        void AddRole(MembershipRole item);
        void DeleteRole(Guid id);
        MembershipUser ValidateUser(string userName, string password);
        IPagedList<MembershipUser> SearchUser(MembershipUserSearchModel searchModel);

        IPagedList<MembershipRole> SearchRole(MembershipRoleSearchModel searchModel);
    }
}
