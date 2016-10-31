using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
namespace XL.CHC.Repositories
{
   public class PermissionRecord_MembershipRole_MappingRepository:IPermissionRecord_MembershipRole_MappingRepository
   {
       private readonly CHCContext _context;
       public PermissionRecord_MembershipRole_MappingRepository(ICHCContext context)
       {
            _context = context as CHCContext;
       }
   }
}
