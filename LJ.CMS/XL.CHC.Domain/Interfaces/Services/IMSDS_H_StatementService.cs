using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IMSDS_H_StatementService
    {
        IList<MSDS_H_Statement> GetAll();
        MSDS_H_Statement Single(string code);
        void Add(MSDS_H_Statement entity);
        void Delete(MSDS_H_Statement entity);
        IList<MSDS_H_Statement> GetListByNames(string[] names);
    }
}
