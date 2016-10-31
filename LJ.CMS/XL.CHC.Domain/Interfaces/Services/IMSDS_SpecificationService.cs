using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IMSDS_SpecificationService
    {
        IList<MSDS_Specification> GetAll();
        void Add(MSDS_Specification specification);
        MSDS_Specification Single(Guid id, Guid company_Id);
        MSDS_Specification Single(string product_Name, Guid company_Id, string supplier_Name);
        void Delete(MSDS_Specification entity);
        IPagedList<MSDS_Specification> Search(SpecificationSearchModel searchModel);
    }
}
