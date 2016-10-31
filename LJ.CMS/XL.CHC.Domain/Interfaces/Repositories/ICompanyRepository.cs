using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface ICompanyRepository
    {
        IList<Company> GetAll();
        Company GetById(Guid companyId);
        IPagedList<Company> Search(CompanySearchModel searchModel);
        Company GetByName(string v);
        void Add(Company company);
    }
}
