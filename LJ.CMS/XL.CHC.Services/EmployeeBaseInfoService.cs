using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class EmployeeBaseInfoService:IEmployeeBaseInfoService
   {
       private readonly IEmployeeBaseInfoRepository _employeeBaseInfoRepository;
       public EmployeeBaseInfoService(IEmployeeBaseInfoRepository employeeBaseInfoRepository)
       {
           _employeeBaseInfoRepository = employeeBaseInfoRepository;
       }

        public EmployeeBaseInfo GetById(Guid id)
        {
            return _employeeBaseInfoRepository.GetById(id);
        }

        public EmployeeBaseInfo GetByIdCard(string idCard)
        {
            return _employeeBaseInfoRepository.GetByIDCard(idCard);
        }

        public void Add(EmployeeBaseInfo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _employeeBaseInfoRepository.Add(entity);
        }
    }
}
