using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class PermissionRecordService:IPermissionRecordService
   {
       private readonly IPermissionRecordRepository _permissionRecordRepository;
       public PermissionRecordService(IPermissionRecordRepository permissionRecordRepository)
       {
           _permissionRecordRepository = permissionRecordRepository;
       }
   }
}
