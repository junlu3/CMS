using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class MembershipUser_MembershipRole_MappingService:IMembershipUser_MembershipRole_MappingService
   {
       private readonly IMembershipUser_MembershipRole_MappingRepository _membershipUser_MembershipRole_MappingRepository;
       public MembershipUser_MembershipRole_MappingService(IMembershipUser_MembershipRole_MappingRepository membershipUser_MembershipRole_MappingRepository)
       {
           _membershipUser_MembershipRole_MappingRepository = membershipUser_MembershipRole_MappingRepository;
       }
   }
}
