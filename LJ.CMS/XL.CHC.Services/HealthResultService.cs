using System;
using System.Collections.Generic;
using System.IO;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
    public class HealthResultService : IHealthResultService
    {
        private readonly IHealthResultRepository _healthResultRepository;
        private readonly IWorkContext _workContext;
        private readonly IImportExportService _importExportService;
        private readonly IEmployeeBaseInfoRepository _employeeBaseInfoRepository;
        private readonly ICompanyEmployeeRepository _companyEmployeeRepository;
        private readonly ICompanyRepository _companyRepository;

        private const string ERROR_IDCARD_EMPTY = "身份证号不能为空";
        private const string ERROR_IDCARD = "无效的身份证号";
        private const string ERROR_USERNOTEXISTS = "受检单位不存在该员工";
        private const string ERROR_1 = "受检单位不能为空";
        private const string ERROR_2 = "无效的日期，格式应为yyyy-MM-dd或yyyy/MM/dd";
        private const string ERROR_3 = "体检单位不能为空";
        private const string ERROR_4 = "主检医师不能为空";
        private const string ERROR_5 = "体检日期不能为空";
        private const string ERROR_COMPANYNOTEXISTS = "系统还没有该受检单位的信息。";


        public HealthResultService(IHealthResultRepository healthResultRepository
            , IWorkContext workContext
            , IImportExportService importExportService
            , IEmployeeBaseInfoRepository employeeBaseInfoRepository
            , ICompanyEmployeeRepository companyEmployeeRepository
            , ICompanyRepository companyRepository
            )
        {
            _healthResultRepository = healthResultRepository;
            _workContext = workContext;
            _importExportService = importExportService;
            _employeeBaseInfoRepository = employeeBaseInfoRepository;
            _companyEmployeeRepository = companyEmployeeRepository;
            _companyRepository = companyRepository;
        }

        public List<string> GetAllFinalResults()
        {
            return _healthResultRepository.GetAllFinalResults();
        }

        #region import

        public ImportResultModel ImportHealthResult(Stream inputStream, bool isCoverData)
        {
            var extraCols = new List<ImportExtraColModel>()
            {
                new ImportExtraColModel() { RowIndex = 2, ColIndex = 2 },
                new ImportExtraColModel() { RowIndex = 3, ColIndex = 2 },
                new ImportExtraColModel() { RowIndex = 4, ColIndex = 2 },
                new ImportExtraColModel() { RowIndex = 5, ColIndex = 2 },
                new ImportExtraColModel() { RowIndex = 6, ColIndex = 2 },
            };
            var healthResultSheet = new ImportSheetModel()
            {
                SheetName = "体检结果",
                HeaderRowIndex = 8,
                StartRowIndex = 9,
                ColCount = 13,
                ExtraCols = extraCols,
                ParseMain = ParseToEntityMain,
                ParseExtra = ParseToEntityExtra,
            };
            var result = _importExportService.ImportWithExtra(inputStream, healthResultSheet, isCoverData);
            result.Title = "体检结果";
            return result;
        }

        private void ParseToEntityExtra(List<string> values, ref List<ImportErrorColModel> errors, bool isCoverData)
        {
            if (string.IsNullOrEmpty(values[0]))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 0,
                    ColName = "受检单位",
                    Message = ERROR_1
                });
            }
            else
            {
                Company company = _companyRepository.GetByName(values[0]);
                if (company == null)
                {
                    errors.Add(new ImportErrorColModel()
                    {
                        ColIndex = 0,
                        ColName = "受检单位",
                        Message = ERROR_COMPANYNOTEXISTS
                    });
                }
            }
            var date = new DateTime();
            if (string.IsNullOrEmpty(values[1]))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 1,
                    ColName = "体检日期",
                    Message = ERROR_5
                });
            }
            else
            {
                if (!DateTime.TryParse(values[1], out date))
                {
                    errors.Add(new ImportErrorColModel()
                    {
                        ColIndex = 1,
                        ColName = "体检日期",
                        Message = ERROR_2
                    });
                }
            }
            //if (string.IsNullOrEmpty(values[2]))
            //{
            //    errors.Add(new ImportErrorColModel()
            //    {
            //        ColIndex = 2,
            //        ColName = "体检单位",
            //        Message = ERROR_3
            //    });
            //}
            //if (string.IsNullOrEmpty(values[3]))
            //{
            //    errors.Add(new ImportErrorColModel()
            //    {
            //        ColIndex = 3,
            //        ColName = "报告日期",
            //        Message = ERROR_5
            //    });
            //}

            //if (string.IsNullOrEmpty(values[4]))
            //{
            //    errors.Add(new ImportErrorColModel()
            //    {
            //        ColIndex = 4,
            //        ColName = "主检医师",
            //        Message = ERROR_4
            //    });
            //}
        }

        private void ParseToEntityMain(List<string> values, ref List<ImportErrorColModel> errors, bool isCoverData)
        {

            //判断源数据是否有身份证号
            if (!string.IsNullOrEmpty(values[2]))
            {
                var company = _companyRepository.GetByName(values[13]);

                //判断是否有companyEmployee
                var companEmployee = _companyEmployeeRepository.GetEmployee(values[2], company.Id);
                if (companEmployee == null)
                {
                    errors.Add(new ImportErrorColModel()
                    {
                        ColIndex = 2,
                        Message = ERROR_USERNOTEXISTS
                    });
                }

                var entity = _healthResultRepository.GetByReportCode(values[12]);
                //创建新体检记录
                if (entity == null)
                {
                    entity = new HealthResult();
                    try
                    {
                        ParseToHealthResult(values, ref entity, errors);
                        if (entity != null)
                        {
                            //是否覆盖
                            if (isCoverData)
                            {
                                _healthResultRepository.DeleteByReportCode(entity.ReportCode);
                            }
                            entity.CreatedBy = _workContext.CurrentMembershipUser.Username;
                            entity.CreatedDate = DateTime.Now;
                            entity.CompanyEmployee = companEmployee;
                            _healthResultRepository.Add(entity);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    try
                    {
                        ParseToHealthResult(values, ref entity, errors);
                        if(entity !=null )
                        {
                            entity.UpdatedBy = _workContext.CurrentMembershipUser.Username;
                            entity.UpdatedDate = DateTime.Now;
                        }
                       
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            else
            {
                //没有身份证号
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 2,
                    Message = ERROR_IDCARD_EMPTY,
                });

            }
        }

        private void ParseToHealthResult(List<string> values, ref HealthResult entity, List<ImportErrorColModel> errors)
        {
            if(errors.Count >0)
            {
                entity = null;
                return;
            }
           
            entity.MainPositiveResult = values[8];
            entity.Result = values[9];
            entity.Deleted = false;
            entity.HealthCode = values[10];
            entity.ImageCode = values[11];
            entity.ReportCode = values[12];
            //values[13]
            if (!string.IsNullOrEmpty(values[14]))
            {
                entity.HealthDate = DateTime.Parse(values[14]);
            }
            if (!string.IsNullOrEmpty(values[15]))
            {
                entity.HealthByCompany = values[15];
            }
            if (!string.IsNullOrEmpty(values[16]))
            {
                entity.ReportDate = values[16];
            }
            entity.HealthPerson = values[17];
        }

        #endregion 
        public IPagedList<HealthResult> Search(HealthResultSearchModel searchModel)
        {
            return _healthResultRepository.Search(searchModel);
        }

        #region Export
        public void ExportHealthResult(string templateFilePath, List<ImportResultModel> data)
        {
            var excelModel = new ExcelModel();
            foreach (var sheet in data)
            {
                var excelSheetModel = new ExcelSheetModel();
                excelSheetModel.Name = sheet.Title;
                bool hasExtra = false;
                for (int row = 0; row < sheet.ErrorRows.Count; row++)
                {
                    if (sheet.ErrorRows[row].RowIndex == 0)
                    {
                        for (var col = 0; col < sheet.ErrorRows[row].Values.Count; col++)
                        {
                            var cell = new ExcelSheetCellModel();
                            cell.RowIndex = col + 2;
                            cell.ColumnIndex = 2;
                            cell.Value = sheet.ErrorRows[row].Values[col];
                            foreach (var error in sheet.ErrorRows[row].ErrorCols)
                            {
                                if (col == error.ColIndex)
                                {
                                    cell.Errors.Add(error.Message);
                                }
                            }
                            excelSheetModel.Cells.Add(cell);
                        }
                        hasExtra = true;
                    }
                    else
                    {
                        for (var col = 0; col < sheet.ErrorRows[row].Values.Count; col++)
                        {
                            var cell = new ExcelSheetCellModel();
                            cell.RowIndex = hasExtra ? row + 8 : row + 9;
                            cell.ColumnIndex = col + 1;
                            cell.Value = sheet.ErrorRows[row].Values[col];
                            foreach (var error in sheet.ErrorRows[row].ErrorCols)
                            {
                                if (col == error.ColIndex)
                                {
                                    cell.Errors.Add(error.Message);
                                }
                            }
                            excelSheetModel.Cells.Add(cell);
                        }
                    }
                }
                excelModel.Sheets.Add(excelSheetModel);
            }
            _importExportService.ExportWithTemplate(templateFilePath, excelModel);

        }

        public void ExportHealthResult(string filePath, List<HealthResult> data)
        {
            var excelModel = new ExcelModel();

            var excelSheet = new ExcelSheetModel();
            excelSheet.Name = "Sheet1";

            if (data.Count > 0)
            {
                var rowIndex = 3;
                foreach (var entity in data)
                {
                    var colIndex = 1;
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "序号",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = (rowIndex - 2).ToString()
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "姓名",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.CompanyEmployee.EmployeeBaseInfo.UserName
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "身份证",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.CompanyEmployee.EmployeeBaseInfo.IDCard
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "性别",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.CompanyEmployee.EmployeeBaseInfo.Sex
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "出生年月",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = Utilities.IDCardHelper.GetBirthDay(entity.CompanyEmployee.EmployeeBaseInfo.IDCard).ToString("yyyy年MM月")
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "工种",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.CompanyEmployee.WorkType
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "接害工龄",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.CompanyEmployee.AdverseMonthes.HasValue ? entity.CompanyEmployee.AdverseMonthes.Value.ToString() : ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "职业病危害因素",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.CompanyEmployee.AdverseFactor
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "检查主要阳性结果",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.MainPositiveResult
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "检查结论",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Result
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检编号",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.HealthCode
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "影像号",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.ImageCode
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "报告号",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.ReportCode
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "主检医师",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.HealthPerson
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "报告日期",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.ReportDate
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检日期",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.HealthDate.HasValue ? entity.HealthDate.Value.ToString("yyyy年MM月") : ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "受检单位",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.CompanyEmployee.Company.CompanyName
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检单位",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.HealthByCompany
                    });
                    rowIndex++;
                }
            }

            excelModel.Sheets.Add(excelSheet);

            _importExportService.ExportWithTemplate(filePath, excelModel);
        }
        #endregion 

        public HealthResult GetById(Guid id)
        {
            return _healthResultRepository.GetById(id);
        }

        public void Add(HealthResult entity)
        {
            _healthResultRepository.Add(entity);
        }
    }

}
