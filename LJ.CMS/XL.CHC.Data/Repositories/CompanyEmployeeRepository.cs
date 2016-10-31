using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using System.Linq;
using System;
using System.Collections.Generic;

namespace XL.CHC.Data.Repositories
{
    public class CompanyEmployeeRepository : ICompanyEmployeeRepository
    {
        private readonly CHCContext _context;

        public CompanyEmployeeRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public CompanyEmployee GetByIDCard(string idCard)
        {
            return _context.CompanyEmployee.Where(x => x.EmployeeBaseInfo.IDCard == idCard).FirstOrDefault();
        }

        public void Add(CompanyEmployee entity)
        {
            _context.CompanyEmployee.Add(entity);
        }

        public IList<string> GetAdverseFactors(Guid companyId)
        {
            return _context.CompanyEmployee.Where(x => x.Company.Id == companyId).Select(x => x.AdverseFactor).Distinct().ToList();
        }

        public IPagedList<CompanyEmployee> Search(CompanyEmployeeSearchModel searchModel)
        {
            string selectedAdverseFactor = null;
            bool adverseFactorCompareNullOrEmpty = false;
            if(searchModel.SelectedAdverseFactor!=null )
            {
                selectedAdverseFactor = searchModel.SelectedAdverseFactor.Replace("NULL", "");
                adverseFactorCompareNullOrEmpty = true;
            }
            string selectedDepartment = null;
            bool departmentCompareNullOrEmpty = false;
            if (searchModel.SelectedDepartment != null)
            {
                selectedDepartment = searchModel.SelectedDepartment.Replace("NULL", "");
                departmentCompareNullOrEmpty = true;
            }
            string selectedWorkType = null;
            bool workTypeCompareEmptyOrNull  = false;
            if (searchModel.SeletedWorkType != null)
            {
                selectedWorkType = searchModel.SeletedWorkType.Replace("NULL", "");
                workTypeCompareEmptyOrNull = true;
            }
            var query = _context.CompanyEmployee
                .Where(x => x.Deleted == false
                        && (searchModel.CompanyId ==null || x.Company.Id == searchModel.CompanyId)
                        && (searchModel.KeyWords == string.Empty
                           || x.EmployeeBaseInfo.UserName == searchModel.KeyWords
                           || x.EmployeeBaseInfo.IDCard == searchModel.KeyWords
                           || x.WorkNumber == searchModel.KeyWords
                           || x.Email == searchModel.KeyWords
                           || x.ContactPhone == searchModel.KeyWords)
                        && (searchModel.SelectedAdverseFactor == null || selectedAdverseFactor.Contains(x.AdverseFactor)
                            || adverseFactorCompareNullOrEmpty ==false || adverseFactorCompareNullOrEmpty==true && string.IsNullOrEmpty(x.AdverseFactor)==true)
                        && (searchModel.SelectedDepartment == null || selectedDepartment.Contains(x.Department)
                            || departmentCompareNullOrEmpty ==false || departmentCompareNullOrEmpty ==true && string.IsNullOrEmpty (x.Department)==true )
                        && (searchModel.SelectedHealthStatus == null || x.HealthStatus.Id == searchModel.SelectedHealthStatus)
                        && (searchModel.SelectedPostStatus == null || (searchModel.SelectedPostStatus == (int?)CompanyEmployeePostStatus.Leave) && x.LeaveDate != null
                           || searchModel.SelectedPostStatus == (int?)CompanyEmployeePostStatus.Stay && x.LeaveDate == null)
                        && (searchModel.SeletedWorkType == null || selectedWorkType.Contains(x.WorkType)
                           || workTypeCompareEmptyOrNull ==false || workTypeCompareEmptyOrNull==true && string.IsNullOrEmpty (x.WorkType))
                      )
                .OrderByDescending(x => x.StartPostDate);
            var count = query.Count();
            var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            return new PagedList<CompanyEmployee>(result, searchModel.PageIndex, searchModel.PageSize, count);
        }

        public IList<string> GetDepartments(Guid companyId)
        {
            return _context.CompanyEmployee.Where(x => x.Company.Id == companyId).Select(x => x.Department).Distinct().ToList();
        }

        public IList<string> GetWorkTypes(Guid? companyId)
        {
            return _context.CompanyEmployee.Where(x => companyId==null ||  x.Company.Id == companyId).Select(x => x.WorkType).Distinct().ToList();
        }

        public void DeleteByCompanyID(Guid companyID)
        {
            var entityList = _context.CompanyEmployee.Where(x => x.Company.Id == companyID);
            foreach (var entity in entityList)
            {
                entity.Deleted = true;
            }
        }

        public CompanyEmployee GetByID(Guid id)
        {
            return _context.CompanyEmployee.FirstOrDefault(x => x.Deleted == false && x.Id == id);
        }

        public CompanyEmployee GetEmployee(string idCard, Guid companyId)
        {
            return _context.CompanyEmployee.FirstOrDefault(x => x.Deleted == false && x.EmployeeBaseInfo.IDCard == idCard && x.Company.Id == companyId);
        }
    }
}
