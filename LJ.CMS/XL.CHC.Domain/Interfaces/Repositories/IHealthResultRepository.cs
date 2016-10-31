using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface IHealthResultRepository
    {
        IPagedList<HealthResult> Search(HealthResultSearchModel searchModel);
        List<string> GetAllFinalResults();
        List<HealthResult> GetByIDCard(string v);
        void DeleteByReportCode(string reportCode);
        void Add(HealthResult entity);
        HealthResult GetByReportCode(string v);
        HealthResult GetById(Guid id);
    }
}
