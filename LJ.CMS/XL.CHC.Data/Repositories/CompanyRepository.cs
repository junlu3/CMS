using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
namespace XL.CHC.Data.Repositories
{
   public class CompanyRepository:ICompanyRepository
   {
       private readonly CHCContext _context;
       public CompanyRepository(ICHCContext context)
       {
            _context = context as CHCContext;
       }

        public void Add(Company company)
        {
            if (_context.Company.FirstOrDefault(x => x.CompanyName == company.CompanyName && x.Deleted == false) != null)
            {
                throw new Exception(string.Format("用户名{0}已经存在！", company.CompanyName));
            }
            _context.Company.Add(company);
        }

        public IList<Company> GetAll()
        {
            return _context.Company.Where(x => x.Deleted == false).ToList();
        }

        public Company GetById(Guid companyId)
        {
            return _context.Company.FirstOrDefault(x =>x.Deleted ==false && x.Id == companyId);
        }

        public Company GetByName(string companyName)
        {
            return _context.Company.FirstOrDefault(x => x.Deleted == false && x.CompanyName == companyName);
        }

        public IPagedList<Company> Search(CompanySearchModel searchModel)
        {
            var query = _context.Company.Where(x => (x.Deleted == false)
                  && (string.IsNullOrEmpty (searchModel.KeyWord ) || x.CompanyName.ToLower().Contains(searchModel.KeyWord )
                      || x.CompanyAddress .ToLower ().Contains (searchModel.KeyWord )
                      || x.ContactPerson.ToLower().Contains(searchModel.KeyWord)
                      || x.ContactPhone.ToLower().Contains (searchModel.KeyWord )
                      || x.LegalPerson.ToLower().Contains(searchModel.KeyWord )
                      //|| x.MembershipUser.Username .ToLower().Contains (searchModel.KeyWord )
                      //|| x.MembershipUser.Email.ToLower().Contains (searchModel.KeyWord )
                      )
                      )
                      .OrderBy(x=>x.CompanyName);
            var count = query.Count();
            var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            return new PagedList<Company>(result, searchModel.PageIndex, searchModel.PageSize, count);
        }
    }
}
