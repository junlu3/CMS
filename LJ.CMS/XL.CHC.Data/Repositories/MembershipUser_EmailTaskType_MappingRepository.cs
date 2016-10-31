using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
namespace XL.CHC.Data.Repositories
{
   public class MembershipUser_EmailTaskType_MappingRepository:IMembershipUser_EmailTaskType_MappingRepository
   {
       private readonly CHCContext _context;
       public MembershipUser_EmailTaskType_MappingRepository(ICHCContext context)
       {
            _context = context as CHCContext;
       }
   }
}
