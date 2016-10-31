using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Services
{
    public class CompanyEmployeeService : ICompanyEmployeeService
    {
        #region Fields

        private readonly ICompanyEmployeeRepository _companyEmployeeRepository;
        private readonly IEmployeeBaseInfoRepository _employeeBaseInfoRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWorkContext _workContext;
        private readonly IImportExportService _importExportService;
        private readonly ICompanyService _companyService;
        private readonly IEmployeeWorkHistoryService _employeeWorkHistoryService;

        private const string ERROR_IDCARD_EMPTY = "身份证号不能为空";
        private const string ERROR_IDCARD = "无效的身份证号";
        private const string ERROR_YEAR = "无效的年份";
        private const string ERROR_MONTH = "无效的月份";
        private const string ERROR_STARTPOST_EMPTY = "上岗时间不能为空";
        private const string ERROR_TOTALWORKMONTHES = "无效的总工龄(月)";
        private const string ERROR_ADVERSEMONTHES = "无效的有害工龄(月)";
        private const string ERROR_MARRIED = "无效的婚姻状态";
        private const string ERROR_HEALTHSTATUS = "无效的体检状态";
        private const string ERROR_MIGRANTWORKER = "无效的民工状态";
        private Company _currentUserCompany;
        //private MembershipUser _currentUser;
        #endregion

        #region Ctor

        public CompanyEmployeeService(ICompanyEmployeeRepository companyEmployeeRepository, ICategoryRepository categoryRepository,
            IWorkContext workContext, IEmployeeBaseInfoRepository employeeBaseInfoRepository, IImportExportService importExportService,
            ICompanyService companyService, IEmployeeWorkHistoryService employeeWorkHistoryService)
        {
            _companyEmployeeRepository = companyEmployeeRepository;
            _employeeBaseInfoRepository = employeeBaseInfoRepository;
            _categoryRepository = categoryRepository;
            _workContext = workContext;
            _importExportService = importExportService;
            _companyService = companyService;
            _employeeWorkHistoryService = employeeWorkHistoryService;
        }

        #endregion

        #region Utilities

        //解析成员工信息，并更新或插入
        private void ParseToEntity(List<string> values, ref List<ImportErrorColModel> errors, bool isCoverData)
        {
            //判断源数据是否有身份证号
            if (!string.IsNullOrEmpty(values[1]))
            {
                //判断是否有EmployeeBaseInfo
                var baseInfo = _employeeBaseInfoRepository.GetByIDCard(values[1]);
                if (baseInfo == null)
                {
                    baseInfo = new EmployeeBaseInfo();
                    try
                    {
                        ParseToEmployeeBaseInfo(values, ref baseInfo, ref errors);

                        if (baseInfo != null)
                        {
                            _employeeBaseInfoRepository.Add(baseInfo);
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
                        ParseToEmployeeBaseInfo(values, ref baseInfo, ref errors);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                //_currentUserCompany = _companyService.GetById(_workContext.CurrentMembershipUser.Companies.FirstOrDefault(x => x.IsDefault == true).Id);
                _currentUserCompany = _companyService.GetById(_workContext.CurrentMembershipUser.Company.Id);
                //这里有问题，应先获取公司，再从公司获取员工
                var entity = _companyEmployeeRepository.GetEmployee(baseInfo.IDCard, _currentUserCompany.Id);
                //创建新员工
                if (entity == null)
                {
                    entity = new CompanyEmployee();
                    try
                    {
                        entity.EmployeeBaseInfo = baseInfo;
                        ParseToCompanyEmployee(values, ref entity, errors);
                        if (entity != null)
                        {
                            //是否覆盖
                            if (isCoverData)
                            {
                                _companyEmployeeRepository.DeleteByCompanyID(entity.Company.Id);
                            }
                            entity.EmployeeBaseInfo = baseInfo;
                            _companyEmployeeRepository.Add(entity);
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
                        entity.EmployeeBaseInfo = baseInfo;
                        ParseToCompanyEmployee(values, ref entity, errors);
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
                //继续查看是否有其他错误
                var entity = new CompanyEmployee();
                try
                {
                    ParseToCompanyEmployee(values, ref entity, errors);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// 解析员工基本信息
        /// </summary>
        private void ParseToEmployeeBaseInfo(List<string> values, ref EmployeeBaseInfo entity, ref List<ImportErrorColModel> errors)
        {
            //检查数据项
            var UserName = string.IsNullOrEmpty(values[0]) ? "" : values[0];
            var IDCard = values[1];
            if (IDCard.Length != 18)
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 1,
                    Message = ERROR_IDCARD,
                });
            }
            var Sex = string.IsNullOrEmpty(values[2]) ? "" : values[2];

            if (errors.Count == 0)
            {
                //开始解析
                if (string.IsNullOrEmpty(entity.IDCard))
                {
                    entity.UserName = UserName;
                    entity.IDCard = IDCard;
                    entity.Sex = Sex;
                    entity.Deleted = false;
                    entity.CreatetDate = DateTime.Now;
                    entity.CreatedBy = _workContext.CurrentMembershipUser.Username;
                }
                else
                {
                    entity.UserName = UserName;
                    entity.Sex = Sex;
                }
            }
            else
            {
                entity = null;
            }
        }

        /// <summary>
        /// 解析员工信息
        /// </summary>
        private void ParseToCompanyEmployee(List<string> values, ref CompanyEmployee entity, List<ImportErrorColModel> errors)
        {
            //检查数据项
            Category Married = _categoryRepository.GetByName(string.IsNullOrEmpty(values[3]) ? "未知" : values[3], "MarriedType");
            if (Married == null)
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 3,
                    Message = ERROR_MARRIED,
                });
            }
            //var Married = string.IsNullOrEmpty(values[3]) ? string.Empty : values[3];
            var EntryDate_Year = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[4]) ? "0" : values[4], out EntryDate_Year))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 4,
                    Message = ERROR_YEAR,
                });
            }
            var EntryDate_Month = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[5]) ? "0" : values[5], out EntryDate_Month))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 5,
                    Message = ERROR_MONTH,
                });
            }
            if (!string.IsNullOrEmpty(values[5]) && (EntryDate_Month < 1 || EntryDate_Month > 12))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 5,
                    Message = ERROR_MONTH,
                });
            }
            var LeaveDate_Year = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[6]) ? "0" : values[6], out LeaveDate_Year))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 6,
                    Message = ERROR_YEAR,
                });
            }
            var LeaveDate_Month = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[7]) ? "0" : values[7], out LeaveDate_Month))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 7,
                    Message = ERROR_MONTH,
                });
            }
            if (!string.IsNullOrEmpty(values[7]) && (LeaveDate_Month < 1 || LeaveDate_Month > 12))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 7,
                    Message = ERROR_MONTH,
                });
            }

            var StartPostDate_Year = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[8]) ? "0" : values[8], out StartPostDate_Year))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 8,
                    Message = ERROR_YEAR,
                });
            }
            if (StartPostDate_Year == 0)
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 8,
                    Message = ERROR_STARTPOST_EMPTY,
                });
            }
            var StartPostDate_Month = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[9]) ? "0" : values[9], out StartPostDate_Month))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 9,
                    Message = ERROR_MONTH,
                });
            }
            if (StartPostDate_Month == 0)
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 9,
                    Message = ERROR_STARTPOST_EMPTY,
                });
            }
            if (!string.IsNullOrEmpty(values[9]) && (StartPostDate_Month < 1 || StartPostDate_Month > 12))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 9,
                    Message = ERROR_MONTH,
                });
            }
            var EndPostDate_Year = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[10]) ? "0" : values[10], out EndPostDate_Year))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 10,
                    Message = ERROR_YEAR,
                });
            }
            var EndPostDate_Month = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[11]) ? "0" : values[11], out EndPostDate_Month))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 11,
                    Message = ERROR_MONTH,
                });
            }
            if (!string.IsNullOrEmpty(values[11]) && (EndPostDate_Month < 1 || EndPostDate_Month > 12))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 11,
                    Message = ERROR_MONTH,
                });
            }
            var AdverseFactor = string.IsNullOrEmpty(values[12]) ? "" : values[12];
            var TotalWorkMonthes = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[13]) ? "0" : values[13], out TotalWorkMonthes))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 13,
                    Message = ERROR_MONTH,
                });
            }
            var AdverseMonthes = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[14]) ? "0" : values[14], out AdverseMonthes))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 14,
                    Message = ERROR_MONTH,
                });
            }
            var WorkNumber = string.IsNullOrEmpty(values[15]) ? "" : values[15];
            var ContactPhone = string.IsNullOrEmpty(values[16]) ? "" : values[16];
            var Department = string.IsNullOrEmpty(values[17]) ? "" : values[17];
            var WorkType = string.IsNullOrEmpty(values[18]) ? "" : values[18];
            //Category HealthStatus = _categoryRepository.GetByName(string.IsNullOrEmpty(values[19]) ? "未知" : values[19], "HealthStatus");
            //if (HealthStatus == null)
            //{
            //    errors.Add(new ImportErrorColModel()
            //    {
            //        ColIndex = 19,
            //        Message = ERROR_HEALTHSTATUS,
            //    });
            //}
            Category MigrantWorker = _categoryRepository.GetByName(string.IsNullOrEmpty(values[20]) ? "未知" : values[20], "YesNotType");
            if (MigrantWorker == null)
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 20,
                    Message = ERROR_MIGRANTWORKER,
                });
            }
            var Leader = string.IsNullOrEmpty(values[21]) ? string.Empty : values[21];
            var ProtectType = string.IsNullOrEmpty(values[22]) ? "" : values[22];
            var Email = string.IsNullOrEmpty(values[23]) ? "" : values[23];

            var StartPostDate = new DateTime(StartPostDate_Year, StartPostDate_Month == 0 ? 1 : StartPostDate_Month, 1);
            DateTime? EndPostDate = null;
            if (EndPostDate_Year != 0)
            {
                EndPostDate = new DateTime(EndPostDate_Year, EndPostDate_Month == 0 ? 1 : EndPostDate_Month, 1);
            }
            DateTime? EntryDate = null;
            if (EntryDate_Year != 0)
                EntryDate = new DateTime(EntryDate_Year, EntryDate_Month == 0 ? 1 : EntryDate_Month, 1);
            DateTime? LeaveDate = null;
            if (LeaveDate_Year != 0)
                LeaveDate = new DateTime(LeaveDate_Year, LeaveDate_Month == 0 ? 1 : LeaveDate_Month, 1);

            if ((EndPostDate != null || StartPostDate != null) && EndPostDate <= StartPostDate)
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 11,
                    Message = "离岗时间必须大于上岗时间",
                });
            }

            if (LeaveDate <= EntryDate)
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 7,
                    Message = "离职时间必须大于入职时间",
                });
            }

            if (StartPostDate < EntryDate)
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 5,
                    Message = "上岗时间不能小于入职时间",
                });
            }

            if ((LeaveDate != null || StartPostDate != null) && LeaveDate <= StartPostDate)
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 7,
                    Message = "离职时间必须大于上岗时间",
                });
            }
            var healthStatusId = 21;
            //没有错误，开始解析
            if (errors.Count == 0)
            {
                entity.Company = _currentUserCompany;
                entity.Married = Married;
                entity.EntryDate = GetDateTime(EntryDate_Year, EntryDate_Month);
                entity.LeaveDate = GetDateTime(LeaveDate_Year, LeaveDate_Month);
                entity.StartPostDate = GetDateTime(StartPostDate_Year, StartPostDate_Month);

                entity.EndPostDate = GetDateTime(EndPostDate_Year, EndPostDate_Month);

                entity.AdverseFactor = AdverseFactor;
                entity.TotalWorkMonthes = TotalWorkMonthes;

                entity.AdverseMonthes = AdverseMonthes;
                entity.WorkNumber = WorkNumber;
                entity.ContactPhone = ContactPhone;
                entity.Department = Department;
                entity.WorkType = WorkType;
                //entity.HealthStatus = HealthStatus;
                //体检状态根据上岗时间及离岗时间自动生成（上岗时间在三个月内自动默认为上岗前，与数据库关联，未做岗前体检的人员超过三个月自动默认为在岗
                if (EndPostDate != null)
                {
                    healthStatusId = 12;
                }
                else
                {
                    if ((DateTime.Now - Convert.ToDateTime(StartPostDate)).Days > 90)
                    {
                        healthStatusId = 11;
                    }
                    else
                    {
                        healthStatusId = 13;
                    }
                }
                entity.HealthStatus = _categoryRepository.GetById(healthStatusId);
                entity.MigrantWorker = MigrantWorker;
                entity.ProtectType = ProtectType;
                entity.Email = Email;
                //职业史
                var lastWork = entity.EmployeeBaseInfo.WorkHistories.OrderByDescending(x => x.EntryDate).FirstOrDefault();
                if (lastWork == null || lastWork.EntryDate > entity.StartPostDate)
                {
                    var workHistory = new EmployeeWorkHistory
                    {
                        WorkType = entity.WorkType,
                        Department = entity.Department,
                        EntryDate = entity.StartPostDate,
                        LeaveDate = entity.EndPostDate,
                        AdverseFactor = entity.AdverseFactor,
                        CompanyName = _currentUserCompany.CompanyName,
                        EmployeeBaseInfo = entity.EmployeeBaseInfo,
                        CreatedBy = _workContext.CurrentMembershipUser.Username,
                        Deleted = false
                    };

                    _employeeWorkHistoryService.Add(workHistory);
                }

                if (entity.EndPostDate != null)
                {
                    if (lastWork != null && entity.StartPostDate == lastWork.EntryDate && entity.Company.CompanyName == lastWork.CompanyName)
                    {
                        lastWork.LeaveDate = entity.EndPostDate;
                    }
                }
                //7、总工龄企业未维护空白项的根据职业史中的入岗时间自动生成，接害工龄企业未维护的，根据职业史中危害因素为非空白的记录推算时间，如企业维护时间的按维护时间为准。
                if (string.IsNullOrEmpty(values[13]))
                {
                    var earliestWorkHisotory = entity.EmployeeBaseInfo.WorkHistories.OrderBy(x => x.EntryDate).FirstOrDefault();
                    if (earliestWorkHisotory != null)
                    {
                        entity.TotalWorkMonthes = Math.Abs(DateTime.Now.Month - entity.StartPostDate.Value.Month) + 12 * (DateTime.Now.Year - entity.StartPostDate.Value.Year);
                    }
                }

                if (string.IsNullOrEmpty(values[14]))
                {
                    var workHisotries = entity.EmployeeBaseInfo.WorkHistories.OrderBy(x => x.EntryDate);
                    int monthes = 0;
                    foreach (var item in workHisotries)
                    {
                        if (!string.IsNullOrEmpty(item.AdverseFactor))
                        {
                            DateTime? leaveDate = null;
                            if (item.LeaveDate == null)
                            {
                                leaveDate = DateTime.Now;
                                monthes += (Math.Abs(leaveDate.Value.Month - item.EntryDate.Value.Month) + 12 * (leaveDate.Value.Year - item.EntryDate.Value.Year));

                            }
                        }
                    }
                    entity.AdverseMonthes = monthes;
                }
            }
            else
            {
                entity = null;
            }
        }

        private DateTime? GetDateTime(int year, int month)
        {
            if (year == 0 && month == 0)
            {
                return null;
            }
            else
            {
                return new DateTime(year, month, 1);
            }
        }

        #endregion

        #region Methods

        public ImportResultModel ImportCompanyEmployees(Stream stream, bool isCoverData)
        {
            var companyEmployeeSheet = new ImportSheetModel()
            {
                SheetName = "员工信息",
                HeaderRowIndex = 1,
                StartRowIndex = 2,
                ColCount = 24,
                ParseMain = ParseToEntity,
            };

            var result = _importExportService.Import(stream, companyEmployeeSheet, isCoverData);
            result.Title = "员工信息";

            return result;
        }

        public void ExportCompanyEmployees(string templateFilePath, List<ImportResultModel> data)
        {
            //将List<ImportResultModel>转成ExcelModel后导出
            var excelModel = new ExcelModel();
            foreach (var sheet in data)
            {
                var excelSheetModel = new ExcelSheetModel();
                excelSheetModel.Name = sheet.Title;
                for (int row = 0; row < sheet.ErrorRows.Count; row++)
                {
                    for (var col = 0; col < sheet.ErrorRows[row].Values.Count; col++)
                    {
                        var cell = new ExcelSheetCellModel();
                        cell.RowIndex = row + 2;
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
                excelModel.Sheets.Add(excelSheetModel);
            }
            //_importExportService.Export(templateFilePath, data);
            _importExportService.ExportWithTemplate(templateFilePath, excelModel);
        }

        public void Test()
        {
            var baseInfo = _employeeBaseInfoRepository.GetByIDCard("513030198512077923");
            var healthStatus = _categoryRepository.GetById(21);
            var married = _categoryRepository.GetById(20);
            var ismigrant = _categoryRepository.GetById(20);
            var entity = new CompanyEmployee
            {
                EmployeeBaseInfo = baseInfo,
                HealthStatus = healthStatus,
                Married = married,
                MigrantWorker = ismigrant,
                Company = _workContext.CurrentMembershipUser.Company,
            };
            _companyEmployeeRepository.Add(entity);
        }

        public IList<string> GetAdverseFactors()
        {
            return _companyEmployeeRepository.GetAdverseFactors(_workContext.CurrentMembershipUser.Company.Id);
        }

        public IPagedList<CompanyEmployee> Search(CompanyEmployeeSearchModel searchModel)
        {
            return _companyEmployeeRepository.Search(searchModel);
        }

        public IList<string> GetDepartments()
        {
            return _companyEmployeeRepository.GetDepartments(_workContext.CurrentMembershipUser.Company.Id);
        }

        public IList<string> GetWorkTypes(Guid? companyId)
        {
            return _companyEmployeeRepository.GetWorkTypes(companyId);
        }



        public CompanyEmployee GetById(Guid id)
        {
            return _companyEmployeeRepository.GetByID(id);
        }

        public void Add(CompanyEmployee entity)
        {
            _companyEmployeeRepository.Add(entity);
        }

        public CompanyEmployee GetEmployee(string idCard, Guid companyId)
        {
            return _companyEmployeeRepository.GetEmployee(idCard, companyId);
        }

        #endregion
    }
}
