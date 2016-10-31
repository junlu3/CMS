using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;


namespace XL.CHC.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;

        public MembershipService(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }


        public void AddRole(MembershipRole item)
        {
            _membershipRepository.AddRole(item);
        }

        public void AddUser(MembershipUser user)
        {
            user.Password = XL.Utilities.MD5Helper.MD5Encrypt(user.Password);
            _membershipRepository.AddUser(user);
        }
 
        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public void DeleteRole(Guid id)
        {
            _membershipRepository.DeleteRole(id);
        }

        public void DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public IList<MembershipUser> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<MembershipRole> GetAllRoles()
        {
            return _membershipRepository.GetAllRoles();
        }
        public IList<MembershipUser> GetAllUsersByComapany(Guid company_Id)
        {
            return _membershipRepository.GetAllUsersByComapany(company_Id);
        }
        public IList<MembershipRole> GetAllRolesByCompany(Guid id)
        {
            return _membershipRepository.GetAllRolesByCompany(id);
        }

        public IList<MembershipUser> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public MembershipRole GetRole(Guid id)
        {
            return _membershipRepository.GetRole(id);
        }

        public MembershipUser GetUser(Guid id)
        {
            return _membershipRepository.GetUser(id);
        }

        public MembershipUser GetUser(string userName)
        {
            return _membershipRepository.GetUser(userName);
        }

        public IPagedList<MembershipUser> SearchUser(MembershipUserSearchModel searchModel)
        {
            return _membershipRepository.SearchUser(searchModel);
        }

        public IPagedList<MembershipRole> SearchRole(MembershipRoleSearchModel searchModel)
        {
            return _membershipRepository.SearchRole(searchModel);
        }

        public MembershipUser ValidateUser(string userName, string password)
        {
            password = XL.Utilities.MD5Helper.MD5Encrypt(password);
            return _membershipRepository.GetUser(userName, password);
        }
    }
}
