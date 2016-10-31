using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class CompanyService:ICompanyService
   {
       private readonly ICompanyRepository _companyRepository;
       public CompanyService(ICompanyRepository companyRepository)
       {
           _companyRepository = companyRepository;
       }

        public void Add(Company company)
        {
            _companyRepository.Add(company);
        }

        public IList<Company> GetAll()
        {
            return _companyRepository.GetAll();
        }

        public Company GetById(Guid companyId)
        {
            return _companyRepository.GetById(companyId);
        }

        public IPagedList<Company> Search(CompanySearchModel searchModel)
        {
            return _companyRepository.Search(searchModel);
        }
    }
}
