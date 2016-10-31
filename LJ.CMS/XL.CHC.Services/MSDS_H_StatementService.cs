using System;
using System.Collections.Generic;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Services
{
    public class MSDS_H_StatementService : IMSDS_H_StatementService
    {
        private readonly IMSDS_H_StatementRepository _H_StatementRepository;
        public MSDS_H_StatementService(IMSDS_H_StatementRepository h_StatementReposiotry)
        {
            _H_StatementRepository = h_StatementReposiotry;
        }

        public void Add(MSDS_H_Statement entity)
        {
            _H_StatementRepository.Add(entity);
        }

        public void Delete(MSDS_H_Statement entity)
        {
            _H_StatementRepository.Delete(entity);
        }

        public IList<MSDS_H_Statement> GetAll()
        {
            return _H_StatementRepository.GetAll();
        }

        public MSDS_H_Statement Single(string code)
        {
            return _H_StatementRepository.Single(code);
        }
        public IList<MSDS_H_Statement> GetListByNames(string[] names)
        {
            return _H_StatementRepository.GetListByNames(names);
        }
    }
}
