using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Web.Models
{
    public class ImportResultViewModel
    {
        public List<ImportResultModel> Results { get; set; } = new List<ImportResultModel>();
    }
}