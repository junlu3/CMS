using System;
using System.Collections.Generic;
using System.IO;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Services
{
    public class EmployeeWorkHistoryService : IEmployeeWorkHistoryService
    {
        #region Fields

        private readonly IEmployeeWorkHistoryRepository _employeeWorkHistoryRepository;
        private readonly IEmployeeBaseInfoRepository _employeeBaseInfoRepository;
        private readonly IWorkContext _workContext;
        private readonly IImportExportService _importExportService;

        private const string ERROR_IDCARD_EMPTY = "身份证号不能为空";
        private const string ERROR_IDCARD = "无效的身份证号";
        private const string ERROR_YEAR = "无效的年份";
        private const string ERROR_MONTH = "无效的月份";

        #endregion

        #region Ctor

        public EmployeeWorkHistoryService(IEmployeeWorkHistoryRepository employeeWorkHistoryRepository,
            IEmployeeBaseInfoRepository employeeBaseInfoRepository, IWorkContext workContext,
            IImportExportService importExportService)
        {
            this._employeeWorkHistoryRepository = employeeWorkHistoryRepository;
            this._employeeBaseInfoRepository = employeeBaseInfoRepository;
            this._workContext = workContext;
            this._importExportService = importExportService;
        }

        #endregion

        #region Utilities

        //解析成员工信息，并更新或插入
        private void ParseToEntity(List<string> values, ref List<ImportErrorColModel> errors, bool isCoverData )
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

                var entity = _employeeWorkHistoryRepository.GetByIDCard(values[1]);
                //创建新员工
                if (entity == null)
                {
                    entity = new EmployeeWorkHistory();
                    try
                    {
                        ParseToEmployeeWorkHistory(values, ref entity, ref errors);
                        if (entity != null)
                        {
                            entity.EmployeeBaseInfo = baseInfo;
                            _employeeWorkHistoryRepository.Add(entity);
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
                        ParseToEmployeeWorkHistory(values, ref entity, ref errors);
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
                var entity = new EmployeeWorkHistory();
                try
                {
                    ParseToEmployeeWorkHistory(values, ref entity, ref errors);
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

            if (errors.Count == 0)
            {
                //开始解析
                if (string.IsNullOrEmpty(entity.IDCard))
                {
                    entity.UserName = UserName;
                    entity.IDCard = IDCard;
                    entity.Deleted = false;
                    entity.CreatetDate = DateTime.Now;
                    entity.CreatedBy = _workContext.CurrentMembershipUser.Username;
                }
                else
                {
                    entity.UserName = UserName;
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
        private void ParseToEmployeeWorkHistory(List<string> values, ref EmployeeWorkHistory entity, ref List<ImportErrorColModel> errors)
        {
            //检查数据项
            var EntryDate_Year = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[2]) ? "0" : values[2], out EntryDate_Year))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 2,
                    Message = ERROR_YEAR,
                });
            }
            var EntryDate_Month = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[3]) ? "0" : values[3], out EntryDate_Month))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 3,
                    Message = ERROR_MONTH,
                });
            }
            var LeaveDate_Year = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[4]) ? "0" : values[4], out LeaveDate_Year))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 4,
                    Message = ERROR_YEAR,
                });
            }
            var LeaveDate_Month = 0;
            if (!int.TryParse(string.IsNullOrEmpty(values[5]) ? "0" : values[5], out LeaveDate_Month))
            {
                errors.Add(new ImportErrorColModel()
                {
                    ColIndex = 5,
                    Message = ERROR_MONTH,
                });
            }
            var CompanyName = string.IsNullOrEmpty(values[6]) ? "" : values[6];
            var Department = string.IsNullOrEmpty(values[7]) ? "" : values[7];
            var WorkType = string.IsNullOrEmpty(values[8]) ? "" : values[8];
            var AdverseFactor = string.IsNullOrEmpty(values[9]) ? "" : values[9];
            var ProtectType = string.IsNullOrEmpty(values[10]) ? "" : values[10];

            //没有错误，开始解析
            if (errors.Count == 0)
            {
                entity.EntryDate = GetDateTime(EntryDate_Year, EntryDate_Month);
                entity.LeaveDate = GetDateTime(LeaveDate_Year, LeaveDate_Month);
                entity.CompanyName = CompanyName;
                entity.Department = Department;
                entity.WorkType = WorkType;
                entity.AdverseFactor = AdverseFactor;
                entity.ProtectType = ProtectType;
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

        public ImportResultModel ImportEmployeeWorkHistories(Stream stream)
        {
            var sheet = new ImportSheetModel()
            {
                SheetName = "职业史",
                HeaderRowIndex = 1,
                StartRowIndex = 2,
                ColCount = 11,
                ParseMain = ParseToEntity,
            };

            var result = _importExportService.Import(stream, sheet, false);
            result.Title = "职业史";

            return result;
        }

        public void Add(EmployeeWorkHistory workHistory)
        {
            _employeeWorkHistoryRepository.Add(workHistory);
        }

        #endregion
    }
}
