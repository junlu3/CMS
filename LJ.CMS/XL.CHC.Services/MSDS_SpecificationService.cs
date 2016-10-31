using System;
using System.Collections.Generic;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Services
{
    public class MSDS_SpecificationService : IMSDS_SpecificationService
    {
        private readonly IMSDS_SpecificationRepository _msds_SpecificationRepository;

        public MSDS_SpecificationService(IMSDS_SpecificationRepository msds_SpecificationRepository)
        {
            _msds_SpecificationRepository = msds_SpecificationRepository;
        }

        public void Add(MSDS_Specification specification)
        {
            _msds_SpecificationRepository.Add(specification);
        }

        public IList<MSDS_Specification> GetAll()
        {
            return _msds_SpecificationRepository.GetAll();
        }

        public MSDS_Specification Single(Guid id, Guid company_Id)
        {
            return _msds_SpecificationRepository.Single(id, company_Id);
        }
        public MSDS_Specification Single(string product_Name, Guid company_Id, string supplier_Name)
        {
            return _msds_SpecificationRepository.Single(product_Name,company_Id,supplier_Name);
        }
        public void Delete(MSDS_Specification entity)
        {
            _msds_SpecificationRepository.Delete(entity);
        }

        public IPagedList<MSDS_Specification> Search(SpecificationSearchModel searchModel)
        {
            return _msds_SpecificationRepository.Search(searchModel);
        }
    }
}
