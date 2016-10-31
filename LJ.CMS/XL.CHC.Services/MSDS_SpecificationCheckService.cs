using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Services
{
    public class MSDS_SpecificationCheckService : IMSDS_SpecificationCheckService
    {
        private readonly IMSDS_SpecificationCheckRepository _msds_SpecificationCheckRepository;
        private readonly IImportExportService _importExportService;
        public MSDS_SpecificationCheckService(IMSDS_SpecificationCheckRepository msds_SpecificationCheckRepository, IImportExportService importExportService)
        {
            _msds_SpecificationCheckRepository = msds_SpecificationCheckRepository;
            _importExportService = importExportService;
        }

        public void Add(MSDS_SpecificationCheck specificationCheck)
        {
            _msds_SpecificationCheckRepository.Add(specificationCheck);
        }

        public void Delete(MSDS_SpecificationCheck entity)
        {
            _msds_SpecificationCheckRepository.Delete(entity);
        }

        public IList<MSDS_SpecificationCheck> GetAll()
        {
            return _msds_SpecificationCheckRepository.GetAll();
        }

        public MSDS_SpecificationCheck Single(Guid id)
        {
            return _msds_SpecificationCheckRepository.Single(id);
        }

        public void ExportSpecificationResult(string filePath, List<MSDS_Specification> data)
        {
            var excelModel = new ExcelModel();

            var excelSheet = new ExcelSheetModel();
            excelSheet.Name = "Sheet1";
            if (data.Count > 0)
            {
                int rowIndex = 3;
                foreach (var entity in data)
                {
                    #region 
                    
                    int colIndex = 1;
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "序号",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = (rowIndex - 2).ToString()
                    });
                    string checkStr = string.Empty;
                    switch (entity.SpecificationCheck?.Status)
                    {
                        case 1:
                            checkStr = "待审批";
                            break;
                        case 2:
                            checkStr = "已审批";
                            break;
                        case 3:
                            checkStr = "被驳回";
                            break;
                        default:
                            checkStr = "未审批";
                            break;
                    }
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "审批状态",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = checkStr
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "产品名",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_Name ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "产品名(中文)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.CN_Name ?? " "
                    });
                    string strState = string.Empty;
                    switch (entity.Product_State)
                    {
                        case 1:
                            strState = "气体";
                            break;
                        case 2:
                            strState = "液体";
                            break;
                        case 3:
                            strState = "固态";
                            break;
                        case 4:
                            strState = "气溶胶";
                            break;
                        case 5:
                            strState = "凝胶";
                            break;
                        case 6:
                            strState = "膏状物";
                            break;
                        case 7:
                            strState = "其他";
                            break;
                        default:
                            strState = "N/A";
                            break;
                    }
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "状态",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = strState
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "CAS号",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.CASCode ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "UN编号",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_UN.HasValue? entity.Product_UN.Value.ToString():" "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "有害化学品",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.UnHazardousChemical.HasValue ? (entity.UnHazardousChemical.Value ? "是" : "否" ) : "否"
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "供应商名称",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Supplier_Name ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "供应商电话",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Supplier_Phone ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "供应商24小时应急电话",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Supplier_UrgencyCall ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "危险性分类",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.GHS_Category ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "警告词",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.GHS_Warning ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "危害简述",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_HazardousDescription ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "防范说明(预防)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.GHS_DefenceDes_Values ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "防范说明(响应)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.GHS_DealDES_Values ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "防范说明(储存)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.GHS_StoreDes_Values ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "爆炸物",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.IsExplosive?"是":"否"
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "易燃物",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.IsFlammable ? "是" : "否"
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "腐蚀",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.IsCorrosive ? "是" : "否"
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "长期危害健康",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.IsHealthHazard ? "是" : "否"
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "剧毒",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.IsToxic ? "是" : "否"
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "氧化剂",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.IsOxidizing ? "是" : "否"
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "高压气体",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.IsGasUnderPressure ? "是" : "否"
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "刺激性",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.IsIrritant ? "是" : "否"
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "环境危害",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.IsDangerousToEnvironment ? "是" : "否"
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "个人防护用品(眼面部)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_Protection_FaceAndEye ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "个人防护用品(手部)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_Protection_Hand ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "个人防护用品(呼吸保护)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_Protection_Breathing ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "个人防护用品(足部)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_Protection_Foot ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "个人防护用品(身体)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_Protection_Body ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "个人防护用品(其他防护)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_Protection_Other ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "急救措施(眼面部沾染)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_ET_FaceAndEye ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "急救措施(皮肤手部沾染)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_ET_SkinAndHand ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "急救措施(吸入)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_ET_Inhalation ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "急救措施(食入)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_ET_Ingestion ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "急救措施(应急消防)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_FireProtection ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "急救措施(泄漏处置)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_LeakageHanding ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "急救措施(作业安全)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_OperationSecure ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "急救措施(储存要求)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_StoreRequirement ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "急救措施(废弃物处置)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_WasteHanding ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "急救措施(备注)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Product_Note ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "运输信息(当地法规)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Policie_Local ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "运输信息(联合国法规)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Policie_UN ?? " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "沸点(液体) ℃",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.BoilingPoint_Liquid.HasValue? entity.BoilingPoint_Liquid.Value.ToString():" "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "闪点 ℃",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.FlashingPoint.HasValue ? entity.FlashingPoint.Value.ToString() : " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "燃烧极限范围(下限)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.BurningLimit_Min.HasValue ? entity.BurningLimit_Min.Value.ToString() : " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "燃烧极限范围(上限)",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.BurningLimit_Max.HasValue ? entity.BurningLimit_Max.Value.ToString() : " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "蒸汽压 mmHg",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.VaporPressure.HasValue ? entity.VaporPressure.Value.ToString() : " "
                    });
                    excelSheet.Cells.Add(new ExcelSheetCellModel()
                    {
                        Title = "创建时间",
                        RowIndex = rowIndex,
                        ColumnIndex = colIndex++,
                        Value = entity.Create_Date.HasValue ? entity.Create_Date.Value.ToString("yyyy-MM-dd HH:mm:ss") : "N/A"
                    });
                    #endregion

                    rowIndex++;
                }
            }

            excelModel.Sheets.Add(excelSheet);

            _importExportService.ExportWithTemplate(filePath, excelModel);
        }
    }
}
