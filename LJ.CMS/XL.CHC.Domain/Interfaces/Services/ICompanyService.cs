using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Services
{
    public interface ICompanyService
    {
        IList<Company> GetAll();
        Company GetById(Guid companyId);
        IPagedList<Company> Search(CompanySearchModel searchModel);
        void Add(Company company);
    }
}
