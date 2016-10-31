using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface IMembershipRepository
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
        IPagedList<MembershipUser> SearchUser(MembershipUserSearchModel searchModel);
        MembershipUser GetUser(string userName, string password);
        IPagedList<MembershipRole> SearchRole(MembershipRoleSearchModel searchModel);
    }
}
