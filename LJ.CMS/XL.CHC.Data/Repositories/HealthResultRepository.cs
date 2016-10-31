using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
namespace XL.CHC.Data.Repositories
{
    public class HealthResultRepository : IHealthResultRepository
    {
        private readonly CHCContext _context;
        public HealthResultRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Add(HealthResult entity)
        {
            _context.HealthResult.Add(entity);
        }

        public void DeleteByReportCode(string reportCode)
        {
            var entity = _context.HealthResult.FirstOrDefault(x =>x.Deleted==false &&  x.ReportCode == reportCode);
            if (entity != null)
            {
                entity.Deleted = false;
            }

        }

        public List<string> GetAllFinalResults()
        {
            return _context.HealthResult.Where(x => x.Deleted == false).Select(x => x.Result).Distinct().ToList();
        }

        public HealthResult GetById(Guid id)
        {
            return _context.HealthResult.FirstOrDefault(x => x.Deleted == false && x.Id == id);
        }

        public List<HealthResult> GetByIDCard(string idCard)
        {
            return _context.HealthResult.Where(e =>e.Deleted == false && e.CompanyEmployee.EmployeeBaseInfo.IDCard == idCard).ToList();
        }

        public HealthResult GetByReportCode(string reportCode)
        {
            return _context.HealthResult.FirstOrDefault(x => x.Deleted ==false && x.ReportCode == reportCode);

        }

        public IPagedList<HealthResult> Search(HealthResultSearchModel searchModel)
        {

            string selectedWorkType = null;
            bool compareEmptyOrNullWorkType = false;
            if(searchModel.SelectedWorkType!=null )
            {
                selectedWorkType = searchModel.SelectedWorkType.Replace("NULL", "");
                compareEmptyOrNullWorkType = true;
            }

            string selectedFinalResult = null;
            bool compareEmptyOrNullFinalResult = false;
            if(searchModel.SelectedFinalResults !=null )
            {
                selectedFinalResult = searchModel.SelectedFinalResults.Replace("NULL", "");
                compareEmptyOrNullFinalResult = true;
            }

            var query = _context.HealthResult
                .Where(x => x.Deleted == false
                  && (searchModel.SelectedCompany == null || x.CompanyEmployee.Company.Id == searchModel.SelectedCompany)
                  && (string.IsNullOrEmpty(searchModel.AdverseFactor) || x.CompanyEmployee.AdverseFactor.Contains(searchModel.AdverseFactor))
                  && (searchModel.MinHealthDate == null || x.HealthDate >= searchModel.MinHealthDate)
                  && (searchModel.MaxHealthDate == null || x.HealthDate <= searchModel.MaxHealthDate)
                  && (searchModel.SelectedWorkType == null || selectedWorkType.Contains(x.CompanyEmployee.WorkType) || (compareEmptyOrNullWorkType==false || compareEmptyOrNullWorkType==true && string.IsNullOrEmpty(x.CompanyEmployee.WorkType)==true))
                  && (searchModel.SelectedFinalResults == null || selectedFinalResult.Contains(x.Result) || (compareEmptyOrNullFinalResult==false || compareEmptyOrNullFinalResult==true && string.IsNullOrEmpty (x.Result)==true))
                  && (string.IsNullOrEmpty(searchModel.HealthResultKeyWord) == true || x.Result.Contains(searchModel.HealthResultKeyWord)
                      || x.ImageCode.Contains(searchModel.HealthResultKeyWord)
                      || x.MainPositiveResult.Contains(searchModel.HealthResultKeyWord)
                      || x.ReportCode.Contains(searchModel.HealthResultKeyWord)
                      || x.HealthCode.Contains(searchModel.HealthResultKeyWord)
                      || x.HealthPerson.Contains(searchModel.HealthResultKeyWord)
                      )
                  && (string.IsNullOrEmpty(searchModel.EmployeeKeyWord) == true
                      || x.CompanyEmployee.EmployeeBaseInfo.UserName.Contains(searchModel.EmployeeKeyWord)
                      || x.CompanyEmployee.EmployeeBaseInfo.IDCard.Contains(searchModel.EmployeeKeyWord)
                      || x.CompanyEmployee.AdverseFactor.Contains(searchModel.EmployeeKeyWord)
                      )
                  && (searchModel.MinAdverseYears == null || x.CompanyEmployee.AdverseMonthes >= searchModel.MinAdverseYears * 12)
                  && (searchModel.MaxAdverseYears == null || x.CompanyEmployee.AdverseMonthes <= searchModel.MaxAdverseYears * 12)
                  ).OrderBy(x => x.ReportCode);
            var count = query.Count();
            var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            return new PagedList<HealthResult>(result, searchModel.PageIndex, searchModel.PageSize, count);
        }

    }
}
