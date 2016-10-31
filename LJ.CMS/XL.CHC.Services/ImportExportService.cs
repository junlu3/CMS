using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Services
{
    public class ImportExportService : IImportExportService
    {
        
        public ImportResultModel Import(Stream stream, ImportSheetModel sheet, bool isCoverData)
        {
            ImportResultModel result = new ImportResultModel();

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets[sheet.SheetName];
                if (worksheet == null)
                {
                    throw new Exception("未找到名为 " + sheet.SheetName + " 的Sheet");
                }

                for (var i = 1; i <= sheet.ColCount; i++)
                {
                    result.HeaderRow.Add(worksheet.Cells[sheet.HeaderRowIndex, i].Value == null ? "" : worksheet.Cells[sheet.HeaderRowIndex, i].Value.ToString().Trim());
                }

                var extraCols = new List<string>();
                if (sheet.ExtraCols != null && sheet.ExtraCols.Count > 0)
                {
                    ProcessExtraData(sheet, isCoverData, result, worksheet, extraCols);
                    if (result.ErrorRows.Count > 0)
                    {
                        result.Result = ImportResult.Failed;
                        return result;

                    }
                }

                ProcessMainData(sheet, isCoverData, result, worksheet, extraCols);
            }

            if (result.ErrorRows.Count > 0)
            {
                result.Result = ImportResult.Failed;
            }
            else
            {
                result.Result = ImportResult.Successful;
            }
            return result;
        }

        private static void ProcessExtraData(ImportSheetModel sheet, bool isCoverData, ImportResultModel result, ExcelWorksheet worksheet, List<string> extraCols)
        {
            foreach (var ex in sheet.ExtraCols)
            {
                extraCols.Add(worksheet.Cells[ex.RowIndex, ex.ColIndex].Value == null ? "" : worksheet.Cells[ex.RowIndex, ex.ColIndex].Value.ToString().Trim());
            }
            var modelExtra = new ImportErrorRowModel();
            var errorsExtra = new List<ImportErrorColModel>();
            try
            {
                sheet.ParseToEntityExtra(extraCols, ref errorsExtra, isCoverData);
                if (errorsExtra.Count > 0)
                {
                    modelExtra.RowIndex = 0;
                    modelExtra.Values = extraCols;
                    modelExtra.ErrorCols = errorsExtra;
                    result.ErrorRows.Add(modelExtra);

                }
            }
            catch (Exception e)
            {
                modelExtra.RowIndex = 0;
                modelExtra.Values = extraCols;
                modelExtra.ErrorCols.Add(new ImportErrorColModel()
                {
                    ColName = "",
                    Message = e.ToString(),
                });
                result.ErrorRows.Add(modelExtra);
            }
        }

        private static void ProcessMainData(ImportSheetModel sheet, bool isCoverData, ImportResultModel result, ExcelWorksheet worksheet, List<string> extraCols)
        {
            int iRow = sheet.StartRowIndex;
            while (true)
            {
                bool allColumnsAreEmpty = true;
                for (var i = 1; i <= sheet.ColCount; i++)
                {
                    if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                    {
                        allColumnsAreEmpty = false;
                        break;
                    }
                }
                if (allColumnsAreEmpty)
                {
                    break;
                }

                List<string> values = new List<string>();
                for (var i = 1; i <= sheet.ColCount; i++)
                {
                    values.Add(worksheet.Cells[iRow, i].Value == null ? "" : worksheet.Cells[iRow, i].Value.ToString().Trim());
                }
                if (extraCols.Count > 0)
                {
                    values.AddRange(extraCols);
                }
                var model = new ImportErrorRowModel();
                var errors = new List<ImportErrorColModel>();
                try
                {
                    sheet.ParseToEntityMain(values, ref errors, isCoverData);
                    if (errors.Count > 0)
                    {
                        model.RowIndex = iRow;
                        if (extraCols.Count > 0)
                        {
                            model.Values = values.Take(sheet.ColCount).ToList();
                        }
                        else
                        {
                            model.Values = values;
                        }
                        model.ErrorCols = errors;
                        result.ErrorRows.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    model.RowIndex = iRow;
                    if (extraCols.Count > 0)
                    {
                        model.Values = values.Take(sheet.ColCount).ToList();
                    }
                    else
                    {
                        model.Values = values;
                    }
                    model.ErrorCols.Add(new ImportErrorColModel()
                    {
                        ColName = "",
                        Message = ex.ToString(),
                    });
                    result.ErrorRows.Add(model);
                }

                iRow++;
            }
        }

        public ImportResultModel ImportWithExtra(Stream stream, ImportSheetModel sheet, bool isCoverData)
        {
            ImportResultModel result = new ImportResultModel();

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets[sheet.SheetName];
                if (worksheet == null)
                {
                    throw new Exception("未找到名为 " + sheet.SheetName + " 的Sheet");
                }

                for (var i = 1; i <= sheet.ColCount; i++)
                {
                    result.HeaderRow.Add(worksheet.Cells[sheet.HeaderRowIndex, i].Value == null ? "" : worksheet.Cells[sheet.HeaderRowIndex, i].Value.ToString().Trim());
                }

                var extraCols = new List<string>();
                if (sheet.ExtraCols != null && sheet.ExtraCols.Count > 0)
                {
                    foreach (var ex in sheet.ExtraCols)
                    {
                        extraCols.Add(worksheet.Cells[ex.RowIndex, ex.ColIndex].Value == null ? "" : worksheet.Cells[ex.RowIndex, ex.ColIndex].Value.ToString().Trim());
                    }
                    var modelExtra = new ImportErrorRowModel();
                    var errorsExtra = new List<ImportErrorColModel>();
                    try
                    {
                        sheet.ParseToEntityExtra(extraCols, ref errorsExtra, isCoverData);
                        if (errorsExtra.Count > 0)
                        {
                            modelExtra.RowIndex = 0;
                            modelExtra.Values = extraCols;
                            modelExtra.ErrorCols = errorsExtra;
                            result.ErrorRows.Add(modelExtra);
                        }
                    }
                    catch (Exception e)
                    {
                        modelExtra.RowIndex = 0;
                        modelExtra.Values = extraCols;
                        modelExtra.ErrorCols.Add(new ImportErrorColModel()
                        {
                            ColName = "",
                            Message = e.ToString(),
                        });
                        result.ErrorRows.Add(modelExtra);
                    }
                }

                //如果extra出错，则读取所有单元格到error里，以便原样输出
                if (result.ErrorRows.Count > 0)
                {
                    int iRow = sheet.StartRowIndex;
                    while (true)
                    {
                        bool allColumnsAreEmpty = true;
                        for (var i = 1; i <= sheet.ColCount; i++)
                        {
                            if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                            {
                                allColumnsAreEmpty = false;
                                break;
                            }
                        }
                        if (allColumnsAreEmpty)
                        {
                            break;
                        }

                        List<string> values = new List<string>();
                        for (var i = 1; i <= sheet.ColCount; i++)
                        {
                            values.Add(worksheet.Cells[iRow, i].Value == null ? "" : worksheet.Cells[iRow, i].Value.ToString().Trim());
                        }

                        var modelRow = new ImportErrorRowModel()
                        {
                            RowIndex = iRow,
                            Values = values,
                            ErrorCols = new List<ImportErrorColModel>()
                            {
                                new ImportErrorColModel()
                                {
                                    ColIndex = -1,
                                }
                            },
                        };
                        result.ErrorRows.Add(modelRow);

                        iRow++;
                    }
                }
                //extra没有错，转实体
                else
                {
                    int iRow = sheet.StartRowIndex;
                    while (true)
                    {
                        bool allColumnsAreEmpty = true;
                        for (var i = 1; i <= sheet.ColCount; i++)
                        {
                            if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                            {
                                allColumnsAreEmpty = false;
                                break;
                            }
                        }
                        if (allColumnsAreEmpty)
                        {
                            break;
                        }

                        List<string> values = new List<string>();
                        for (var i = 1; i <= sheet.ColCount; i++)
                        {
                            values.Add(worksheet.Cells[iRow, i].Value == null ? "" : worksheet.Cells[iRow, i].Value.ToString().Trim());
                        }
                        if (extraCols.Count > 0)
                        {
                            values.AddRange(extraCols);
                        }
                        var model = new ImportErrorRowModel();
                        var errors = new List<ImportErrorColModel>();
                        try
                        {
                            sheet.ParseToEntityMain(values, ref errors, isCoverData);
                            if (errors.Count > 0)
                            {
                                model.RowIndex = iRow;
                                if (extraCols.Count > 0)
                                {
                                    model.Values = values.Take(sheet.ColCount).ToList();
                                }
                                else
                                {
                                    model.Values = values;
                                }
                                model.ErrorCols = errors;
                                result.ErrorRows.Add(model);
                            }
                        }
                        catch (Exception ex)
                        {
                            model.RowIndex = iRow;
                            if (extraCols.Count > 0)
                            {
                                model.Values = values.Take(sheet.ColCount).ToList();
                            }
                            else
                            {
                                model.Values = values;
                            }
                            model.ErrorCols.Add(new ImportErrorColModel()
                            {
                                ColName = "",
                                Message = ex.ToString(),
                            });
                            result.ErrorRows.Add(model);
                        }

                        iRow++;
                    }
                }
            }

            if (result.ErrorRows.Count > 0)
            {
                result.Result = ImportResult.Failed;
            }
            else
            {
                result.Result = ImportResult.Successful;
            }
            return result;
        }

        public void ExportWithTemplate(string templateFilePath, ExcelModel model)
        {
            FileInfo fileInfo = new FileInfo(templateFilePath);
            if (!fileInfo.Exists)
            {
                throw new Exception("未找到名为 " + templateFilePath + " 的模板");
            }
            using (var xlPackage = new ExcelPackage(fileInfo))
            {
                foreach (var sheet in model.Sheets)
                {
                    var worksheet = xlPackage.Workbook.Worksheets[sheet.Name];
                    if (worksheet == null)
                    {
                        throw new Exception("模板中未包含名为 " + sheet.Name + " 的Sheet");
                    }

                    foreach (var cell in sheet.Cells)
                    {
                        if (cell.ValueType == ExcelValueType.Image)
                        {
                            ExcelPicture picture = worksheet.Drawings.AddPicture("Stamp", Image.FromFile(cell.Value));//插入图片
                            picture.SetPosition(cell.RowIndex, 0, cell.ColumnIndex, 0);//设置图片的位置
                            picture.SetSize(200, 200);
                        }
                        else if (cell.ValueType == ExcelValueType.String)
                        {
                            worksheet.Cells[cell.RowIndex, cell.ColumnIndex].Value = cell.Value;
                            if (cell.Errors.Count > 0)
                            {
                                worksheet.Cells[cell.RowIndex, cell.ColumnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[cell.RowIndex, cell.ColumnIndex].Style.Fill.BackgroundColor.SetColor(Color.Red);
                            }
                        }
                    }
                }

                xlPackage.Save();
            }
        }

        public void ExportTagWithTemplate(Tag_ExportModel model)
        {
            FileInfo fileInfo = new FileInfo(model.templateFilePath);
            if (!fileInfo.Exists)
            {
                throw new Exception("未找到名为 " + model.templateFilePath + " 的模板");
            }
            using (var xlPackage = new ExcelPackage(fileInfo))
            {

                var worksheet = xlPackage.Workbook.Worksheets[model.SheetName_Big];
                if (worksheet == null)
                {
                    throw new Exception("模板中未包含名为 " + model.SheetName_Big + " 的Sheet");
                }

                #region 大标签
                var listTitle = worksheet.Drawings.Where(o => o.Name == "Title").ToList();
                foreach (var item in listTitle)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.ProductName;
                }

                var listHS = worksheet.Drawings.Where(o => o.Name == "HS").ToList();
                foreach (var item in listHS)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.HS;
                }

                var listWarning = worksheet.Drawings.Where(o => o.Name == "txtWarning").ToList();
                foreach (var item in listWarning)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.WarningContent;
                }

                var listHazardouDes = worksheet.Drawings.Where(o => o.Name == "HazardouDes").ToList();
                foreach (var item in listHazardouDes)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.HazardousDescription;
                }

                var listDefenceDes = worksheet.Drawings.Where(o => o.Name == "DefenceDes").ToList();
                foreach (var item in listDefenceDes)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.DefenceDes;
                }

                var listDealDES = worksheet.Drawings.Where(o => o.Name == "DealDES").ToList();
                foreach (var item in listDealDES)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.DealDES;
                }

                var listStoreDes = worksheet.Drawings.Where(o => o.Name == "StoreDes").ToList();
                foreach (var item in listStoreDes)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.StoreDes;
                }

                var listWasteHanding = worksheet.Drawings.Where(o => o.Name == "WasteHanding").ToList();
                foreach (var item in listWasteHanding)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.WasteHanding;
                }

                var listSupplierBig = worksheet.Drawings.Where(o => o.Name == "Supplier").ToList();
                foreach (var item in listSupplierBig)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.Supplier_Big;
                }


                #endregion

                var worksheet_small = xlPackage.Workbook.Worksheets[model.SheetName_Small];
                if (worksheet_small == null)
                {
                    throw new Exception("模板中未包含名为 " + model.SheetName_Small + " 的Sheet");
                }

                #region 小标签
                var listTitleSmall = worksheet_small.Drawings.Where(o => o.Name == "Title").ToList();
                foreach (var item in listTitleSmall)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.ProductName;
                }

                var listHSSmall = worksheet_small.Drawings.Where(o => o.Name == "HS").ToList();
                foreach (var item in listHSSmall)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.HS;
                }

                var listHazardouDesSmall = worksheet_small.Drawings.Where(o => o.Name == "HazardouDes").ToList();
                foreach (var item in listHazardouDesSmall)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.HazardousDescription;
                }

                var listSupplierSmall = worksheet_small.Drawings.Where(o => o.Name == "Supplier").ToList();
                foreach (var item in listSupplierSmall)
                {
                    ExcelShape shape = item as ExcelShape;
                    shape.RichText.Text = model.Supplier_Small;
                }
                #endregion

                #region 替换警示标识
                int cellCount = model.WarningPicPaths.Count;

                if (cellCount == 1)
                {
                    #region  
                    
                    string pName = "Picture3";
                    var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                    foreach (var item in listA)
                    {
                        var img = item as ExcelPicture;
                        img.Image = Image.FromFile(model.WarningPicPaths[0]);
                    }

                    var listB = worksheet_small.Drawings.Where(o => o.Name == pName).ToList();
                    foreach (var item in listB)
                    {
                        var img = item as ExcelPicture;
                        img.Image = Image.FromFile(model.WarningPicPaths[0]);
                    }

                    for (int i = 1; i < 6; i++)
                    {
                        if (i == 3)
                        {
                            continue;
                        }
                        string _pName = "Picture" + i;
                        var _listA = worksheet.Drawings.Where(o => o.Name == _pName).ToList();
                        foreach (var item in _listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.BlankPicPath);
                        }

                        var _listB = worksheet_small.Drawings.Where(o => o.Name == _pName).ToList();
                        foreach (var item in _listB)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.BlankPicPath);
                        }
                    }
                    #endregion
                }
                else if (cellCount == 2)
                {
                    string pName = "";
                    #region
                    for (int i = 0; i < cellCount; i++)
                    {
                        if (i== 0)
                        {
                            pName = "Picture4";
                        }
                        else
                        {
                            pName = "Picture2";
                        }
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.WarningPicPaths[i]);
                        }

                        var listB = worksheet_small.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listB)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.WarningPicPaths[i]);
                        }
                    }
                    for (int i = 1; i < 6; i++)
                    {
                        if (i == 2 || i == 4)
                        {
                            continue;
                        }
                        pName = "Picture" + i;
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.BlankPicPath);
                        }

                        var listB = worksheet_small.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listB)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.BlankPicPath);
                        }
                    }
                    #endregion
                }
                else if (cellCount == 3)
                {
                    #region
                    string pName = "";

                    for (int i = 0; i < cellCount; i++)
                    {
                        if (i == 0)
                        {
                            pName = "Picture4";
                        }
                        else if (i == 1)
                        {
                            pName = "Picture3";
                        }
                        else
                        {
                            pName = "Picture2";
                        }
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.WarningPicPaths[i]);
                        }

                        var listB = worksheet_small.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listB)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.WarningPicPaths[i]);
                        }
                    }
                    for (int i = 1; i < 6; i++)
                    {
                        if (i == 2 || i == 4 || i == 3)
                        {
                            continue;
                        }
                        pName = "Picture" + i;
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.BlankPicPath);
                        }

                        var listB = worksheet_small.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listB)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.BlankPicPath);
                        }
                    }
                    #endregion
                }
                else if (cellCount == 4)
                {
                    #region
                    string pName = "";

                    for (int i = 0; i < cellCount; i++)
                    {
                        if (i == 0)
                        {
                            pName = "Picture5";
                        }
                        else if (i == 1)
                        {
                            pName = "Picture4";
                        }
                        else if(i == 2)
                        {
                            pName = "Picture3";
                        }
                        else
                        {
                            pName = "Picture2";
                        }
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.WarningPicPaths[i]);
                        }

                        var listB = worksheet_small.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listB)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.WarningPicPaths[i]);
                        }
                    }
                    for (int i = 1; i < 2; i++)
                    {
                        if (i == 1)
                        {
                            pName = "Picture" + i;
                            var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                            foreach (var item in listA)
                            {
                                var img = item as ExcelPicture;
                                img.Image = Image.FromFile(model.BlankPicPath);
                            }

                            var listB = worksheet_small.Drawings.Where(o => o.Name == pName).ToList();
                            foreach (var item in listB)
                            {
                                var img = item as ExcelPicture;
                                img.Image = Image.FromFile(model.BlankPicPath);
                            }
                        }
                    }
                    #endregion
                }
                else if (cellCount == 5)
                {
                    int k = 1;
                    foreach (string picUrl in model.WarningPicPaths)
                    {
                        string pName = "Picture" + k;
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(picUrl);
                        }

                        var listB = worksheet_small.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listB)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(picUrl);
                        }
                        k++;
                    }

                    for (int i = k; i < 6; i++)
                    {
                        string pName = "Picture" + i;
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.BlankPicPath);
                        }

                        var listB = worksheet_small.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listB)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.BlankPicPath);
                        }
                    }
                }

                #endregion

                xlPackage.Save();
            }

        }

        public void ExportTagWithTemplate(Tag_SecurityNotification model)
        {
            FileInfo fileInfo = new FileInfo(model.templateFilePath);
            if (!fileInfo.Exists)
            {
                throw new Exception("未找到名为 " + model.templateFilePath + " 的模板");
            }
            using (var xlPackage = new ExcelPackage(fileInfo))
            {
                var worksheet = xlPackage.Workbook.Worksheets[model.SheetName];
                if (worksheet == null)
                {
                    throw new Exception("模板中未包含名为 " + model.SheetName + " 的Sheet");
                }

                worksheet.Cells["C6"].RichText.Text = model.ProductName;
                worksheet.Cells["C9"].RichText.Text = model.HazardousDescription;

                worksheet.Cells["E14"].RichText.Text = model.Product_ET_FaceAndEye;
                worksheet.Cells["E15"].RichText.Text = model.Product_ET_SkinAndHand;
                worksheet.Cells["E16"].RichText.Text = model.Product_ET_Inhalation;
                worksheet.Cells["E17"].RichText.Text = model.Product_ET_Ingestion;

                #region 警示标识
                int warningCount = model.WarningPicPaths.Count;
                if (warningCount == 1)
                {
                    #region  
                    string pName = "Picture2";
                    var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                    foreach (var item in listA)
                    {
                        var img = item as ExcelPicture;
                        img.Image = Image.FromFile(model.WarningPicPaths[0]);
                    }

                    for (int i = 1; i < 6; i++)
                    {
                        if (i == 2)
                        {
                            continue;
                        }
                        string _pName = "Picture" + i;
                        var _listA = worksheet.Drawings.Where(o => o.Name == _pName).ToList();
                        foreach (var item in _listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.BlankPicPath);
                        }
                    }
                    #endregion
                }
                else if (warningCount == 2)
                {
                    string pName = "";
                    #region
                    for (int i = 0; i < warningCount; i++)
                    {
                        if (i == 0)
                        {
                            pName = "Picture2";
                        }
                        else
                        {
                            pName = "Picture3";
                        }
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.WarningPicPaths[i]);
                        }

                    }
                    for (int i = 1; i < 6; i++)
                    {
                        if (i == 2 || i == 3)
                        {
                            continue;
                        }
                        pName = "Picture" + i;
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.BlankPicPath);
                        }
                    }
                    #endregion
                }
                else if (warningCount == 3)
                {
                    #region
                    string pName = "";

                    for (int i = 0; i < warningCount; i++)
                    {
                        if (i == 0)
                        {
                            pName = "Picture1";
                        }
                        else if (i == 1)
                        {
                            pName = "Picture2";
                        }
                        else
                        {
                            pName = "Picture3";
                        }
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.WarningPicPaths[i]);
                        }

                    }
                    for (int i = 1; i < 6; i++)
                    {
                        if (i == 1 || i == 2 || i == 3)
                        {
                            continue;
                        }
                        pName = "Picture" + i;
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.BlankPicPath);
                        }

                    }
                    #endregion
                }
                else if (warningCount == 4)
                {
                    #region
                    string pName = "";

                    for (int i = 0; i < warningCount; i++)
                    {
                        if (i == 0)
                        {
                            pName = "Picture2";
                        }
                        else if (i == 1)
                        {
                            pName = "Picture3";
                        }
                        else if (i == 2)
                        {
                            pName = "Picture4";
                        }
                        else
                        {
                            pName = "Picture5";
                        }
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(model.WarningPicPaths[i]);
                        }

                    }
                    for (int i = 1; i < 2; i++)
                    {
                        if (i == 1)
                        {
                            pName = "Picture" + i;
                            var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                            foreach (var item in listA)
                            {
                                var img = item as ExcelPicture;
                                img.Image = Image.FromFile(model.BlankPicPath);
                            }

                        }
                    }
                    #endregion
                }
                else if (warningCount == 5)
                {
                    int j = 1;
                    foreach (string picUrl in model.WarningPicPaths)
                    {
                        string pName = "Picture" + j;
                        var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                        foreach (var item in listA)
                        {
                            var img = item as ExcelPicture;
                            img.Image = Image.FromFile(picUrl);
                        }

                        j++;
                    }
                }
                #endregion

                #region 个人防护
                int protectCount = model.ProtectionPicPaths.Count;

                int k = 1;
                foreach (string picUrl in model.ProtectionPicPaths)
                {
                    string pName = "PictureProtection" + k;
                    var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                    foreach (var item in listA)
                    {
                        var img = item as ExcelPicture;
                        img.Image = Image.FromFile(picUrl);
                    }

                    k++;
                }

                for (int i = k; i < 7; i++)
                {
                    string pName = "PictureProtection" + i;
                    var listA = worksheet.Drawings.Where(o => o.Name == pName).ToList();
                    foreach (var item in listA)
                    {
                        var img = item as ExcelPicture;
                        img.Image = Image.FromFile(model.BlankPicPath);
                    }

                }
                #endregion
                xlPackage.Save();
            }
        }
        public void ExportWithTemplateExtend(string templateFilePath, ExcelModel model)
        {
            FileInfo fileInfo = new FileInfo(templateFilePath);
            if (!fileInfo.Exists)
            {
                throw new Exception("未找到名为 " + templateFilePath + " 的模板");
            }
            using (var xlPackage = new ExcelPackage(fileInfo))
            {
                foreach (var sheet in model.Sheets)
                {
                    var worksheet = xlPackage.Workbook.Worksheets[sheet.Name];
                    if (worksheet == null)
                    {
                        throw new Exception("模板中未包含名为 " + sheet.Name + " 的Sheet");
                    }

                    List<int> templateRows = new List<int>();
                    foreach (var cell in sheet.Cells)
                    {
                        if (cell.TemplateRowIndex != -1)
                        {
                            if (!templateRows.Contains(cell.TemplateRowIndex))
                            {
                                templateRows.Add(cell.TemplateRowIndex);
                            }
                            worksheet.InsertRow(cell.RowIndex, 1);
                            worksheet.Cells[cell.TemplateRowIndex, 1, cell.TemplateRowIndex, 100].Copy(worksheet.Cells[cell.RowIndex, 1, cell.RowIndex, 100]);
                        }

                        worksheet.Cells[cell.RowIndex, cell.ColumnIndex].Value = cell.Value;
                        if (cell.Errors.Count > 0)
                        {
                            worksheet.Cells[cell.RowIndex, cell.ColumnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[cell.RowIndex, cell.ColumnIndex].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        }
                    }

                    if (templateRows.Count > 0)
                    {
                        templateRows.Sort();
                        for (int i = templateRows.Count - 1; i >= 0;  i--)
                        {
                            worksheet.DeleteRow(templateRows[i], 1, true);
                        }
                    }
                }

                xlPackage.Save();
            }
        }




    }
}
