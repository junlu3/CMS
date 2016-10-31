using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class MembershipUser_Company_MappingService:IMembershipUser_Company_MappingService
   {
       private readonly IMembershipUser_Company_MappingRepository _membershipUser_Company_MappingRepository;
       public MembershipUser_Company_MappingService(IMembershipUser_Company_MappingRepository membershipUser_Company_MappingRepository)
       {
           _membershipUser_Company_MappingRepository = membershipUser_Company_MappingRepository;
       }
   }
}
