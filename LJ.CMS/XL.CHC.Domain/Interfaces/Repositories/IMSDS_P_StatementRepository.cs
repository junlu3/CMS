using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface IMSDS_P_StatementRepository
    {
        IList<MSDS_P_Statement> GetAll();
        MSDS_P_Statement Single(string code);
        void Add(MSDS_P_Statement entity);
        void Delete(MSDS_P_Statement entity);
        IList<MSDS_P_Statement> GetListByNames(string[] names);
    }
}
