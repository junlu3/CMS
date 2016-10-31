using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface IMSDS_SpecificationCheckRepository
    {
        IList<MSDS_SpecificationCheck> GetAll();
        void Add(MSDS_SpecificationCheck specification);
        MSDS_SpecificationCheck Single(Guid id);
        void Delete(MSDS_SpecificationCheck entity);
    }
}
