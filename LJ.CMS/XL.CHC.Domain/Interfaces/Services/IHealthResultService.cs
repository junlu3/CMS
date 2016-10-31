using System;
using System.Collections.Generic;
using System.IO;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IHealthResultService
    {
        List<string> GetAllFinalResults();
        IPagedList<HealthResult> Search(HealthResultSearchModel searchModel);
        ImportResultModel ImportHealthResult(Stream inputStream, bool isCoverData);
        void ExportHealthResult(string filePath, List<ImportResultModel> results);

        void ExportHealthResult(string filePath, List<HealthResult> data);
        HealthResult GetById(Guid id);
        void Add(HealthResult entity);
    }
}
