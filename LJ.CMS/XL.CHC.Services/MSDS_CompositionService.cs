using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Services
{
    public class MSDS_CompositionService : IMSDS_CompositionService
    {
        private readonly IMSDS_CompositionRepository _repository;
        public MSDS_CompositionService(IMSDS_CompositionRepository repository)
        {
            _repository = repository;
        }
        public void Add(MSDS_Composition entity)
        {
            _repository.Add(entity);
        }

        public IList<MSDS_Composition> GetAll()
        {
            return _repository.GetAll();
        }
    }
}
