using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface IMSDS_H_StatementRepository
    {
        IList<MSDS_H_Statement> GetAll();
        MSDS_H_Statement Single(string code);
        void Add(MSDS_H_Statement entity);
        void Delete(MSDS_H_Statement entity);
        IList<MSDS_H_Statement> GetListByNames(string[] names);
    }
}
