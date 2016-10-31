using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class PermissionRecord_MembershipRole_MappingService:IPermissionRecord_MembershipRole_MappingService
   {
       private readonly IPermissionRecord_MembershipRole_MappingRepository _permissionRecord_MembershipRole_MappingRepository;
       public PermissionRecord_MembershipRole_MappingService(IPermissionRecord_MembershipRole_MappingRepository permissionRecord_MembershipRole_MappingRepository)
       {
           _permissionRecord_MembershipRole_MappingRepository = permissionRecord_MembershipRole_MappingRepository;
       }
   }
}
