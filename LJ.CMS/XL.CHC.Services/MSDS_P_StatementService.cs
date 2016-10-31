using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Domain.Interfaces.Repositories;

namespace XL.CHC.Services
{
    public class MSDS_P_StatementService : IMSDS_P_StatementService
    {
        public readonly IMSDS_P_StatementRepository _repository;

        public MSDS_P_StatementService(IMSDS_P_StatementRepository repository)
        {
            _repository = repository;
        }
        public void Add(MSDS_P_Statement entity)
        {
            _repository.Add(entity);
        }

        public void Delete(MSDS_P_Statement entity)
        {
            _repository.Delete(entity);
        }

        public IList<MSDS_P_Statement> GetAll()
        {
            return _repository.GetAll();
        }

        public IList<MSDS_P_Statement> GetListByNames(string[] names)
        {
            return _repository.GetListByNames(names);
        }

        public MSDS_P_Statement Single(string code)
        {
            return _repository.Single(code);
        }
    }
}
