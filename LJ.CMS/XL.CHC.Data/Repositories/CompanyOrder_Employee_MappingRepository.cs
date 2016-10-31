using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
namespace XL.CHC.Repositories
{
   public class CompanyOrder_Employee_MappingRepository:ICompanyOrder_Employee_MappingRepository
   {
       private readonly CHCContext _context;
       public CompanyOrder_Employee_MappingRepository(ICHCContext context)
       {
            _context = context as CHCContext;
       }
   }
}
