using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;

namespace XL.CHC.Data.Repositories
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly CHCContext _context;
        public MembershipRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void AddRole(MembershipRole item)
        {
            _context.MembershipRole.Add(item);
        }

        public void AddUser(MembershipUser user)
        {
            if (_context.MembershipUser.FirstOrDefault(x => x.Username == user.Username && x.Deleted == false) != null)
            {
                throw new Exception(string.Format("用户名{0}已经存在！", user.Username));
            }
            _context.MembershipUser.Add(user);
        }

        public void DeleteRole(Guid id)
        {
            var entity = _context.MembershipRole.SingleOrDefault(x=>x.Id == id);
            if (entity != null)
            {
                _context.MembershipRole.Remove(entity);
            }
            
        }

        public void DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public IList<MembershipRole> GetAllRoles()
        {
            return _context.MembershipRole.Where(x=> !x.Deleted).ToList();
        }

        public IList<MembershipRole> GetAllRolesByCompany(Guid id)
        {
            return _context.MembershipRole.Where(x=> !x.Deleted && x.Company.Id == id).ToList<MembershipRole>();
        }

        public IList<MembershipUser> GetAllUsers()
        {
            throw new NotImplementedException();
        }
        public IList<MembershipUser> GetAllUsersByComapany(Guid company_Id)
        {
            return _context.MembershipUser.Where(x=>x.Company.Id == company_Id).ToList<MembershipUser>();
        }

        public MembershipRole GetRole(Guid id)
        {
            return _context.MembershipRole.FirstOrDefault(x => x.Id == id);
        }

        public MembershipUser GetUser(Guid id)
        {
            return _context.MembershipUser.FirstOrDefault(x => x.Id == id);
        }

        public MembershipUser GetUser(string userName)
        {
            return _context.MembershipUser
                .FirstOrDefault(x => x.Username.ToLower() == userName.ToLower());
        }

        public MembershipUser GetUser(string userName, string password)
        {
            return _context.MembershipUser.FirstOrDefault(x => x.Deleted == false
                                    && (x.Username == userName || x.Email == userName)
                                    && x.Password == password);
        }

        public IPagedList<MembershipUser> SearchUser(MembershipUserSearchModel searchModel)
        {
            if (searchModel.Roles.Any(x => x.Id == new Guid("7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f")))
            {
                var query = _context.MembershipUser.Where(x => (x.Deleted == false)
                    && (string.IsNullOrEmpty(searchModel.KeyWord) || x.Username.ToLower().Contains(searchModel.KeyWord)
                     || x.Email.ToLower().Contains(searchModel.KeyWord)
                     ))
                 .OrderBy(x => x.Username);
                var count = query.Count();
                var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
                return new PagedList<MembershipUser>(result, searchModel.PageIndex, searchModel.PageSize, count);
            }
            else
            {
                var query = _context.MembershipUser.Where(x => (x.Deleted == false)
                 && x.Company.Id == searchModel.Company_Id
                 && (string.IsNullOrEmpty(searchModel.KeyWord) || x.Username.ToLower().Contains(searchModel.KeyWord)
                 || x.Email.ToLower().Contains(searchModel.KeyWord)
                 ))
                 .OrderBy(x => x.Username);
                var count = query.Count();
                var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
                return new PagedList<MembershipUser>(result, searchModel.PageIndex, searchModel.PageSize, count);
            }

        }

        public IPagedList<MembershipRole> SearchRole(MembershipRoleSearchModel searchModel)
        {
            var query = _context.MembershipRole.Where(x => (string.IsNullOrEmpty(searchModel.KeyWord) || x.Name.ToLower().Contains(searchModel.KeyWord)) && x.Deleted == false && x.Company.Id == searchModel.Company_Id)
            .OrderBy(x => x.Name);
            var count = query.Count();
            var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            return new PagedList<MembershipRole>(result, searchModel.PageIndex, searchModel.PageSize, count);

            //if (searchModel.Roles.Any(x => x.Id == new Guid("7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f")))
            //{
            //    #region 拥有超级管理员的权限
            //    var query = _context.MembershipRole.Where(x => (string.IsNullOrEmpty(searchModel.KeyWord) || x.Name.ToLower().Contains(searchModel.KeyWord)) && x.Deleted == false)
            //     .OrderBy(x => x.Company.CompanyName);
            //    var count = query.Count();
            //    var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            //    return new PagedList<MembershipRole>(result, searchModel.PageIndex, searchModel.PageSize, count);
            //    #endregion
            //}
            //else
            //{
            //    var query = _context.MembershipRole.Where(x => (string.IsNullOrEmpty(searchModel.KeyWord) || x.Name.ToLower().Contains(searchModel.KeyWord)) && x.Deleted == false && x.Company.Id == searchModel.Company_Id)
            //     .OrderBy(x => x.Name);
            //    var count = query.Count();
            //    var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            //    return new PagedList<MembershipRole>(result, searchModel.PageIndex, searchModel.PageSize, count);
            //}
        }
    }
}
