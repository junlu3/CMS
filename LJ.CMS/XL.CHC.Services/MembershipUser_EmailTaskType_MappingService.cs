using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class MembershipUser_EmailTaskType_MappingService:IMembershipUser_EmailTaskType_MappingService
   {
       private readonly IMembershipUser_EmailTaskType_MappingRepository _membershipUser_EmailTaskType_MappingRepository;
       public MembershipUser_EmailTaskType_MappingService(IMembershipUser_EmailTaskType_MappingRepository membershipUser_EmailTaskType_MappingRepository)
       {
           _membershipUser_EmailTaskType_MappingRepository = membershipUser_EmailTaskType_MappingRepository;
       }
   }
}
