using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
namespace XL.CHC.Data.Repositories
{
   public class EmployeeBaseInfoRepository:IEmployeeBaseInfoRepository
   {
       private readonly CHCContext _context;
       public EmployeeBaseInfoRepository(ICHCContext context)
       {
            _context = context as CHCContext;
       }

        public EmployeeBaseInfo GetByIDCard(string idCard)
        {
            return _context.EmployeeBaseInfo.Where(x => x.IDCard == idCard).FirstOrDefault();
        }

        public void Add(EmployeeBaseInfo entity)
        {
            _context.EmployeeBaseInfo.Add(entity);
        }

        public EmployeeBaseInfo GetById(Guid id)
        {
            return _context.EmployeeBaseInfo.FirstOrDefault(x => x.Deleted == false && x.Id == id);
        }
    }
}
