using System.Collections.Generic;
using System.IO;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IImportExportService
    {
        ImportResultModel Import(Stream stream, ImportSheetModel sheet, bool isCoverData);

        ImportResultModel ImportWithExtra(Stream stream, ImportSheetModel sheet, bool isCoverData);

        void ExportWithTemplate(string templateFilePath, ExcelModel model);
        void ExportTagWithTemplate(Tag_ExportModel model);
        void ExportTagWithTemplate(Tag_SecurityNotification model);
        void ExportWithTemplateExtend(string templateFilePath, ExcelModel model);
    }
}
