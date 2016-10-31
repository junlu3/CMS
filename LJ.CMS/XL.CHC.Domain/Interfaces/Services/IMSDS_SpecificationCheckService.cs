using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IMSDS_SpecificationCheckService
    {
        IList<MSDS_SpecificationCheck> GetAll();
        void Add(MSDS_SpecificationCheck specification);
        MSDS_SpecificationCheck Single(Guid id);
        void Delete(MSDS_SpecificationCheck entity);
        void ExportSpecificationResult(string filePath, List<MSDS_Specification> data);
    }
}
