using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IMSDS_CompositionService
    {
        void Add(MSDS_Composition entity);
        IList<MSDS_Composition> GetAll();
    }
}
