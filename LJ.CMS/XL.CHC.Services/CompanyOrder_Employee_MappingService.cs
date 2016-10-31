using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class CompanyOrder_Employee_MappingService:ICompanyOrder_Employee_MappingService
   {
       private readonly ICompanyOrder_Employee_MappingRepository _companyOrder_Employee_MappingRepository;
       public CompanyOrder_Employee_MappingService(ICompanyOrder_Employee_MappingRepository companyOrder_Employee_MappingRepository)
       {
           _companyOrder_Employee_MappingRepository = companyOrder_Employee_MappingRepository;
       }
   }
}
