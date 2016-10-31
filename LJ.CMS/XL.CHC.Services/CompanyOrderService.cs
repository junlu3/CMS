using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
using XL.Utilities;

namespace XL.CHC.Services
{
    public class CompanyOrderService : ICompanyOrderService
    {
        #region Fields

        private readonly ICompanyOrderRepository _companyOrderRepository;
        private readonly IImportExportService _importExportService;
        private readonly IHealthResultRepository _healthResultRepository;
        private readonly IEmailService _emailService;

        private readonly static string AGREEMENTChANGED_EMAIL_FORMATTER = "您预约日期为{0}的订单协议已被医院更新,请核查。";

        #endregion

        #region Ctor

        public CompanyOrderService(ICompanyOrderRepository companyOrderRepository, IImportExportService importExportService,
            IHealthResultRepository healthResultRepository
            , IEmailService emailService)
        {
            _companyOrderRepository = companyOrderRepository;
            _importExportService = importExportService;
            _healthResultRepository = healthResultRepository;
            _emailService = emailService;
        }

        #endregion

        #region Methods

        public void Add(CompanyOrder order)
        {
            _companyOrderRepository.Add(order);
        }

        public CompanyOrder GetById(Guid orderId)
        {
            return _companyOrderRepository.GetById(orderId);
        }

        public IPagedList<CompanyOrder> Search(CompanyOrderSearchModel searchModel)
        {
            return _companyOrderRepository.Search(searchModel);
        }

        #region Export

        public void ExportCompanyOrderForms(string webRootPath, CompanyOrder order)
        {
            var folder = webRootPath + @"Content\ExportFiles\Orders\" + order.Id.ToString();

            //如果有同名文件夹，删除
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            //创建文件夹
            Directory.CreateDirectory(folder);

            if (order.CompanyEmployees.Count > 0)
            {
                //导出检查表
                List<string> checkForms = new List<string>();
                foreach (var emp in order.CompanyEmployees)
                {
                    var templateFilePath = webRootPath + @"\Content\Templates\" + "上海市职业健康检查表.xlsx";
                    var targetFilePath = folder + @"\上海市职业健康检查表_" + emp.EmployeeBaseInfo.UserName + ".xlsx";
                    File.Copy(templateFilePath, targetFilePath);
                    ExportCheckForm(targetFilePath, emp, webRootPath);
                    checkForms.Add(targetFilePath);
                }

                //压缩检查表
                //ZipHelper.CompressFiles(folder + @"\上海市职业健康检查表.zip", checkForms);

                //删除检查表
                //foreach (var path in checkForms)
                //{
                //    File.Delete(path);
                //}

                //导出告知书
                List<string> adverseFactorNoticeForms = new List<string>();
                foreach (var emp in order.CompanyEmployees)
                {
                    var templateFilePath = webRootPath + @"\Content\Templates\" + "职业病危害因素告知书.xlsx";
                    var targetFilePath = folder + @"\职业病危害因素告知书_" + emp.EmployeeBaseInfo.UserName + ".xlsx";
                    File.Copy(templateFilePath, targetFilePath);
                    ExportAdverseFactorNoticeForm(targetFilePath, emp);
                    adverseFactorNoticeForms.Add(targetFilePath);
                }

                //压缩告知书
                //ZipHelper.CompressFiles(folder + @"\职业病危害因素告知书.zip", adverseFactorNoticeForms);

                //删除告知书
                //foreach (var path in adverseFactorNoticeForms)
                //{
                //    File.Delete(path);
                //}
            }

            //导出登记表
            var templateFilePath1 = webRootPath + @"\Content\Templates\" + "上海市职业健康检查应检者登记表.xlsx";
            var targetFilePath1 = folder + @"\上海市职业健康检查应检者登记表.xlsx";
            File.Copy(templateFilePath1, targetFilePath1);
            ExportCheckerRegistrationForm(targetFilePath1, order);

            //导出造册表
            var templateFilePath2 = webRootPath + @"\Content\Templates\" + "上海市作业人员职业病危害因素接触情况造册表.xlsx";
            var targetFilePath2 = folder + @"\上海市作业人员职业病危害因素接触情况造册表.xlsx";
            File.Copy(templateFilePath2, targetFilePath2);
            ExportAdverseFactorContactSituationRegistrationForm(targetFilePath2, order);

            //导出协议书
            var templateFilePath3 = webRootPath + @"\Content\Templates\" + "协议书.xlsx";
            var targetFilePath3 = folder + @"\协议书.xlsx";
            File.Copy(templateFilePath3, targetFilePath3);
            ExportProtocol(targetFilePath3, order);
        }

        public void ExportCheckForm(string filePath, CompanyEmployee emp, string webRootPath = "")
        {
            var excelModel = new ExcelModel();

            var excelSheet = new ExcelSheetModel();
            excelSheet.Name = "Sheet1";

            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "编号",
                RowIndex = 3,
                ColumnIndex = 5,
                Value = DateTime.Now.ToString("yyyyMMddhhmmss"),
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "姓名",
                RowIndex = 5,
                ColumnIndex = 2,
                Value = emp.EmployeeBaseInfo.UserName
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "性别",
                RowIndex = 5,
                ColumnIndex = 5,
                Value = emp.EmployeeBaseInfo.Sex
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "身份证号码",
                RowIndex = 7,
                ColumnIndex = 2,
                Value = emp.EmployeeBaseInfo.IDCard,
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "婚姻状况",
                RowIndex = 7,
                ColumnIndex = 5,
                Value = emp.Married.Name
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "总工龄",
                RowIndex = 9,
                ColumnIndex = 2,
                Value = emp.TotalWorkMonthes.HasValue ? emp.TotalWorkMonthes.Value.ToString() : "?"
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "接害工龄",
                RowIndex = 9,
                ColumnIndex = 5,
                Value = emp.AdverseMonthes.HasValue ? emp.AdverseMonthes.Value.ToString() : "?"
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "毒害种类和名称",
                RowIndex = 12,
                ColumnIndex = 1,
                Value = emp.AdverseFactor
            });
            if (!string.IsNullOrEmpty(emp.Company.CompanyStamp))
            {
                excelSheet.Cells.Add(new ExcelSheetCellModel()
                {
                    Title = "用人单位签章",
                    RowIndex = 12,
                    ColumnIndex = 4,
                    Value = webRootPath + emp.Company.CompanyStamp.Replace("/", "\\"),
                    ValueType = ExcelValueType.Image,
                });
            }

            if (emp.EmployeeBaseInfo.WorkHistories != null && emp.EmployeeBaseInfo.WorkHistories.Count > 0)
            {
                var startRow = 21;
                foreach (var history in emp.EmployeeBaseInfo.WorkHistories)
                {
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "起止日期",
                        RowIndex = startRow,
                        ColumnIndex = 1,
                        Value = string.Format("{0}-{1}",
                        history.EntryDate.HasValue ? history.EntryDate.Value.ToString("yyyy-MM-dd") : "?",
                        history.LeaveDate.HasValue ? history.LeaveDate.Value.ToString("yyyy-MM-dd") : "?")
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "工作单位",
                        RowIndex = startRow,
                        ColumnIndex = 2,
                        Value = history.CompanyName
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "车间",
                        RowIndex = startRow,
                        ColumnIndex = 3,
                        Value = history.Department
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "工种",
                        RowIndex = startRow,
                        ColumnIndex = 4,
                        Value = history.WorkType
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "有害因素",
                        RowIndex = startRow,
                        ColumnIndex = 5,
                        Value = history.AdverseFactor
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "防护措施",
                        RowIndex = startRow,
                        ColumnIndex = 6,
                        Value = history.ProtectType
                    });
                    startRow++;
                }
            }

            excelModel.Sheets.Add(excelSheet);

            _importExportService.ExportWithTemplate(filePath, excelModel);
        }

        public void ExportCheckerRegistrationForm(string filePath, CompanyOrder order)
        {
            var excelModel = new ExcelModel();

            var excelSheet = new ExcelSheetModel();
            excelSheet.Name = "Sheet1";

            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "单位",
                RowIndex = 3,
                ColumnIndex = 2,
                Value = order.Company.CompanyName
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "地址",
                RowIndex = 5,
                ColumnIndex = 2,
                Value = order.Company.CompanyAddress
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "邮编",
                RowIndex = 5,
                ColumnIndex = 5,
                Value = order.Company.ZipCode
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "电话",
                RowIndex = 7,
                ColumnIndex = 2,
                Value = order.Company.ContactPhone
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "传真",
                RowIndex = 7,
                ColumnIndex = 5,
                Value = order.Company.Fax
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "双休日",
                RowIndex = 9,
                ColumnIndex = 2,
                Value = order.Company.RestDay
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "法定代表人",
                RowIndex = 9,
                ColumnIndex = 5,
                Value = order.Company.LegalPerson
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "登记日期",
                RowIndex = 11,
                ColumnIndex = 2,
                Value = order.Company.CreatedDate.HasValue ? order.Company.CreatedDate.Value.ToString("yyyy年MM月dd日") : ""
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "单位性质",
                RowIndex = 11,
                ColumnIndex = 5,
                Value = order.Company.CompanyType == null ? string.Empty : order.Company.CompanyType.Name
            });
            if (order.CompanyEmployees != null && order.CompanyEmployees.Count > 0)
            {
                var emps = order.CompanyEmployees.GroupBy(e => new { e.Department, e.WorkType, e.HealthStatus, e.AdverseFactor })
                            .Select(g => (new { key = g.Key, count = g.Count() }));

                var startRow = 14;
                foreach (var emp in emps)
                {
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "车间",
                        RowIndex = startRow,
                        ColumnIndex = 1,
                        Value = emp.key.Department
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "工种",
                        RowIndex = startRow,
                        ColumnIndex = 2,
                        Value = emp.key.WorkType
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检类别",
                        RowIndex = startRow,
                        ColumnIndex = 3,
                        Value = emp.key.HealthStatus.Name
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "职业危害因素",
                        RowIndex = startRow,
                        ColumnIndex = 4,
                        Value = emp.key.AdverseFactor
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "接触人数",
                        RowIndex = startRow,
                        ColumnIndex = 5,
                        Value = emp.count.ToString()
                    });
                    startRow++;
                }
            }

            excelModel.Sheets.Add(excelSheet);

            _importExportService.ExportWithTemplate(filePath, excelModel);
        }

        public void ExportAdverseFactorContactSituationRegistrationForm(string filePath, CompanyOrder order)
        {
            var excelModel = new ExcelModel();

            var excelSheet = new ExcelSheetModel();
            excelSheet.Name = "Sheet1";

            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "单位名称",
                RowIndex = 3,
                ColumnIndex = 3,
                Value = order.Company.CompanyName
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "单位地址",
                RowIndex = 4,
                ColumnIndex = 3,
                Value = order.Company.CompanyAddress
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "邮编",
                RowIndex = 4,
                ColumnIndex = 10,
                Value = order.Company.ZipCode
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "法人",
                RowIndex = 5,
                ColumnIndex = 3,
                Value = order.Company.LegalPerson
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "联系人",
                RowIndex = 5,
                ColumnIndex = 7,
                Value = order.Company.ContactPerson
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "联系电话",
                RowIndex = 5,
                ColumnIndex = 10,
                Value = order.Company.ContactPhone
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "休息日",
                RowIndex = 6,
                ColumnIndex = 3,
                Value = order.Company.RestDay
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "企业注册类型",
                RowIndex = 6,
                ColumnIndex = 7,
                Value = order.Company.CompanyRegisterType == null ? string.Empty : order.Company.CompanyRegisterType.Name
            });

            if (order.CompanyEmployees != null && order.CompanyEmployees.Count > 0)
            {
                var row = 9;
                foreach (var emp in order.CompanyEmployees)
                {
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "序号",
                        RowIndex = row,
                        ColumnIndex = 1,
                        Value = (row - 8).ToString()
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "姓名",
                        RowIndex = row,
                        ColumnIndex = 2,
                        Value = emp.EmployeeBaseInfo.UserName
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "性别",
                        RowIndex = row,
                        ColumnIndex = 3,
                        Value = emp.EmployeeBaseInfo.Sex
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "年龄",
                        RowIndex = row,
                        ColumnIndex = 4,
                        Value = IDCardHelper.GetAge(emp.EmployeeBaseInfo.IDCard).ToString()
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "民工",
                        RowIndex = row,
                        ColumnIndex = 5,
                        Value = emp.MigrantWorker.Name
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "车间部门",
                        RowIndex = row,
                        ColumnIndex = 6,
                        Value = emp.Department
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "工种",
                        RowIndex = row,
                        ColumnIndex = 7,
                        Value = emp.WorkType
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "危害因素",
                        RowIndex = row,
                        ColumnIndex = 8,
                        Value = emp.AdverseFactor
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "接触工龄",
                        RowIndex = row,
                        ColumnIndex = 9,
                        Value = emp.AdverseMonthes.HasValue ? emp.AdverseMonthes.ToString() : ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "检查类别",
                        RowIndex = row,
                        ColumnIndex = 10,
                        Value = emp.HealthStatus.Name
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "备注",
                        RowIndex = row,
                        ColumnIndex = 11,
                        Value = emp.Comment
                    });

                    row++;
                }
            }

            excelModel.Sheets.Add(excelSheet);

            _importExportService.ExportWithTemplate(filePath, excelModel);
        }

        public void ExportProtocol(string filePath, CompanyOrder order)
        {
            var excelModel = new ExcelModel();

            var excelSheet = new ExcelSheetModel();
            excelSheet.Name = "Sheet1";

            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "公司名",
                RowIndex = 2,
                ColumnIndex = 3,
                Value = order.Company.CompanyName
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "公司地址",
                RowIndex = 3,
                ColumnIndex = 3,
                Value = order.Company.CompanyAddress
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "体检时间",
                RowIndex = 10,
                ColumnIndex = 2,
                Value = XL.Utilities.StringHelper.Join(order.SubOrders.Select(x => x.StartDate.Value.ToString("yyyy-MM-dd")).ToArray(), ";")
            });

            excelModel.Sheets.Add(excelSheet);
            _importExportService.ExportWithTemplate(filePath, excelModel);
        }

        public void ExportAdverseFactorNoticeForm(string filePath, CompanyEmployee emp)
        {
            var excelModel = new ExcelModel();

            var excelSheet = new ExcelSheetModel();
            excelSheet.Name = "Sheet1";

            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "姓名",
                RowIndex = 2,
                ColumnIndex = 1,
                Value = emp.EmployeeBaseInfo.UserName
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "车间",
                RowIndex = 4,
                ColumnIndex = 3,
                Value = emp.Department
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "岗位",
                RowIndex = 4,
                ColumnIndex = 5,
                Value = emp.WorkType
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "危害因素",
                RowIndex = 5,
                ColumnIndex = 2,
                Value = emp.AdverseFactor
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "防护用品",
                RowIndex = 11,
                ColumnIndex = 1,
                Value = emp.ProtectType
            });

            excelModel.Sheets.Add(excelSheet);

            _importExportService.ExportWithTemplate(filePath, excelModel);
        }

        public void ExportHealthCareRecords(string filePath, CompanyEmployee emp)
        {
            var baseRowTemplateIndex_Table2 = 21;
            var baseRowTemplateIndex_Table3 = 26;
            var baseRowTemplateIndex_Table4 = 31;
            var baseRowTemplateIndex_Table5 = 46;
            var totalMoveRowCount = 0;

            var excelModel = new ExcelModel();

            var excelSheet = new ExcelSheetModel();
            excelSheet.Name = "Sheet1";

            #region 封面

            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "编号",
                RowIndex = 1,
                ColumnIndex = 16,
                Value = DateTime.Now.ToString("yyyyMMddhhmmss"),
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "区县",
                RowIndex = 2,
                ColumnIndex = 16,
                Value = "",
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "姓名",
                RowIndex = 7,
                ColumnIndex = 7,
                Value = emp.EmployeeBaseInfo.UserName,
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "单位名称",
                RowIndex = 8,
                ColumnIndex = 7,
                Value = emp.Company.CompanyName,
            });

            #endregion

            #region 基本信息

            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "工号",
                RowIndex = 12,
                ColumnIndex = 2,
                Value = emp.WorkNumber,
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "岗位状态",
                RowIndex = 12,
                ColumnIndex = 9,
                Value = emp.WorkType
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "上岗时间",
                RowIndex = 12,
                ColumnIndex = 15,
                Value = emp.StartPostDate.HasValue ? emp.StartPostDate.Value.ToString("yyyy/MM") : "",
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "离岗时间",
                RowIndex = 12,
                ColumnIndex = 17,
                Value = emp.EndPostDate.HasValue ? emp.EndPostDate.Value.ToString("yyyy/MM") : "",
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "姓名",
                RowIndex = 13,
                ColumnIndex = 2,
                Value = emp.EmployeeBaseInfo.UserName,
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "身份证",
                RowIndex = 13,
                ColumnIndex = 9,
                Value = emp.EmployeeBaseInfo.IDCard,
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "性别",
                RowIndex = 13,
                ColumnIndex = 15,
                Value = emp.EmployeeBaseInfo.Sex,
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "是否需离岗体检",
                RowIndex = 13,
                ColumnIndex = 17,
                Value = "",
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "出生年月",
                RowIndex = 14,
                ColumnIndex = 2,
                Value = IDCardHelper.GetBirthDay(emp.EmployeeBaseInfo.IDCard).ToString("yyyy年MM月"),
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "户籍",
                RowIndex = 14,
                ColumnIndex = 9,
                Value = "",
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "是否已体检",
                RowIndex = 14,
                ColumnIndex = 17,
                Value = "",
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "文化程度",
                RowIndex = 15,
                ColumnIndex = 2,
                Value = "",
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "是否吸烟",
                RowIndex = 15,
                ColumnIndex = 9,
                Value = "",
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "个人爱好",
                RowIndex = 16,
                ColumnIndex = 2,
                Value = "",
            });
            excelSheet.Cells.Add(new ExcelSheetCellModel()
            {
                Title = "既往史",
                RowIndex = 17,
                ColumnIndex = 2,
                Value = "",
            });

            #endregion

            #region 职业史

            if (emp.EmployeeBaseInfo.WorkHistories.Count > 0)
            {
                for (int i = 0; i < emp.EmployeeBaseInfo.WorkHistories.Count; i++)
                {
                    var item = emp.EmployeeBaseInfo.WorkHistories[i];
                    var rowIndex = baseRowTemplateIndex_Table2 + i + 1;

                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "用人单位名称",
                        RowIndex = rowIndex,
                        ColumnIndex = 1,
                        Value = item.CompanyName,
                        TemplateRowIndex = baseRowTemplateIndex_Table2
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "从事工种",
                        RowIndex = rowIndex,
                        ColumnIndex = 3,
                        Value = item.WorkType,
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "工作岗位",
                        RowIndex = rowIndex,
                        ColumnIndex = 6,
                        Value = item.Department,
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "起始时间",
                        RowIndex = rowIndex,
                        ColumnIndex = 8,
                        Value = string.Format("{0} - {1}", item.EntryDate.HasValue ? item.EntryDate.Value.ToString("yyyy年MM月") : "", item.LeaveDate.HasValue ? item.LeaveDate.Value.ToString("yyyy年MM月") : "")
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "是否危害岗位",
                        RowIndex = rowIndex,
                        ColumnIndex = 14,
                        Value = string.IsNullOrEmpty(item.AdverseFactor) ? "否" : "是",
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "接触职业危害因素名称1",
                        RowIndex = rowIndex,
                        ColumnIndex = 15,
                        Value = item.AdverseFactor,
                    });
                }

                totalMoveRowCount += emp.EmployeeBaseInfo.WorkHistories.Count;
            }

            #endregion

            #region 职业危害接触史

            baseRowTemplateIndex_Table3 += totalMoveRowCount;
            totalMoveRowCount += 0;

            #endregion

            #region 职业卫生（健康）检查结果

            baseRowTemplateIndex_Table4 += totalMoveRowCount;
            var healthResultList = _healthResultRepository.GetByIDCard(emp.EmployeeBaseInfo.IDCard);
            if (healthResultList.Count > 0)
            {
                for (int i = 0; i < healthResultList.Count; i++)
                {
                    var item = healthResultList[i];
                    var rowIndex = baseRowTemplateIndex_Table4 + i + 1;

                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "检查时间",
                        RowIndex = rowIndex,
                        ColumnIndex = 1,
                        Value = item.HealthDate.HasValue ? item.HealthDate.Value.ToString("yyyy年MM月") : "",
                        TemplateRowIndex = baseRowTemplateIndex_Table4
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检项目-粉尘",
                        RowIndex = rowIndex,
                        ColumnIndex = 2,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检项目-毒物",
                        RowIndex = rowIndex,
                        ColumnIndex = 3,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检项目-物理因素",
                        RowIndex = rowIndex,
                        ColumnIndex = 4,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "健康监护类型-上岗前",
                        RowIndex = rowIndex,
                        ColumnIndex = 5,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "健康监护类型-在岗期间",
                        RowIndex = rowIndex,
                        ColumnIndex = 6,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "健康监护类型-离岗时",
                        RowIndex = rowIndex,
                        ColumnIndex = 7,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检结果-正常",
                        RowIndex = rowIndex,
                        ColumnIndex = 8,
                        Value = item.Result
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检结果-复查",
                        RowIndex = rowIndex,
                        ColumnIndex = 9,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检结果-确诊",
                        RowIndex = rowIndex,
                        ColumnIndex = 10,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检结果-禁忌",
                        RowIndex = rowIndex,
                        ColumnIndex = 11,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检结果-疑似",
                        RowIndex = rowIndex,
                        ColumnIndex = 12,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检项目-其他病患",
                        RowIndex = rowIndex,
                        ColumnIndex = 13,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "复查时间",
                        RowIndex = rowIndex,
                        ColumnIndex = 14,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "复查情况",
                        RowIndex = rowIndex,
                        ColumnIndex = 15,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "复查结论",
                        RowIndex = rowIndex,
                        ColumnIndex = 16,
                        Value = ""
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "体检单位",
                        RowIndex = rowIndex,
                        ColumnIndex = 17,
                        Value = item.HealthByCompany
                    });
                }

                totalMoveRowCount += healthResultList.Count;
            }

            #endregion

            #region 职业病诊疗情况

            baseRowTemplateIndex_Table5 += totalMoveRowCount;

            #endregion

            excelModel.Sheets.Add(excelSheet);

            _importExportService.ExportWithTemplateExtend(filePath, excelModel);
        }

        #endregion

        public void Delete(string orderId)
        {
            var entity = _companyOrderRepository.GetById(new Guid(orderId));
            if (entity == null)
            {
                throw new Exception("未找到编号为 " + orderId + " 的订单");
            }
            else
            {
                _companyOrderRepository.Delete(entity);
            }
        }

        public void SendEmailWhileUploadAgreement(Guid id)
        {
            var order = _companyOrderRepository.GetById(id);
            foreach (var item in order.Company.MembershipUsers)
            {
                if (item.EmailTaskTypes.FirstOrDefault(x=>x.Id == 29) != null)
                {
                    string emailBody = string.Empty;
                    var minOrderDate = order.SubOrders.OrderBy(x => x.StartDate).FirstOrDefault().StartDate;
                    var maxOrderDate = order.SubOrders.OrderBy(x => x.StartDate).LastOrDefault().StartDate;

                    if (maxOrderDate > minOrderDate)
                    {
                        emailBody = string.Format(AGREEMENTChANGED_EMAIL_FORMATTER, Convert.ToDateTime(minOrderDate).ToString("yyyy-MM-dd") + "~" + Convert.ToDateTime(maxOrderDate).ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        emailBody = string.Format(AGREEMENTChANGED_EMAIL_FORMATTER, Convert.ToDateTime(minOrderDate).ToString("yyyy-MM-dd"));
                    }
                    var email = new Email
                    {
                        To = item.Email,
                        ToName = order.Company.CompanyName,
                        Body = string.Format(EmailBodyFormatter.EnterpriseBody, emailBody)
                    };
                    _emailService.Add(email);
                }
            }
        }

        #endregion
    }
}
