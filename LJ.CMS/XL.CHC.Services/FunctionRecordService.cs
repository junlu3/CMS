using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class FunctionRecordService:IFunctionRecordService
   {
       private readonly IFunctionRecordRepository _functionRecordRepository;
       public FunctionRecordService(IFunctionRecordRepository functionRecordRepository)
       {
           _functionRecordRepository = functionRecordRepository;
       }
   }
}
