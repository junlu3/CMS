using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class MembershipRoleService:IMembershipRoleService
   {
       private readonly IMembershipRoleRepository _membershipRoleRepository;
       public MembershipRoleService(IMembershipRoleRepository membershipRoleRepository)
       {
           _membershipRoleRepository = membershipRoleRepository;
       }
   }
}
