using System.Collections.Generic;

namespace XL.CHC.Domain.DomainModel
{
    #region Common

    public class ExcelModel
    {
        public List<ExcelSheetModel> Sheets { get; set; } = new List<ExcelSheetModel>();
    }

    public class ExcelSheetModel
    {
        public string Name { get; set; } = string.Empty;

        public List<ExcelSheetCellModel> Cells { get; set; } = new List<ExcelSheetCellModel>();
    }

    public class ExcelSheetCellModel
    {
        public int RowIndex { get; set; } = 0;

        public int ColumnIndex { get; set; } = 0;

        public string Title { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public List<string> Errors { get; set; } = new List<string>();

        public int TemplateRowIndex { get; set; } = -1;

        public ExcelValueType ValueType { get; set; } = ExcelValueType.String;
    }

    public enum ExcelValueType
    {
        String,
        Image
    }

    #endregion

    #region Import

    public class ImportSheetModel
    {
        #region Fields

        public string SheetName { get; set; } = "";

        public int HeaderRowIndex { get; set; } = 1;

        public int StartRowIndex { get; set; } = 2;

        public int ColCount { get; set; } = 1;

        public List<ImportExtraColModel> ExtraCols { get; set; }

        public ParseToEntityDelegate ParseMain { get; set; }

        public ParseToEntityDelegate ParseExtra { get; set; }

        #endregion

        #region Delegates

        public delegate void ParseToEntityDelegate(List<string> values, ref List<ImportErrorColModel> errors, bool isCoverData);

        #endregion

        public void ParseToEntityMain(List<string> values, ref List<ImportErrorColModel> errors, bool isCoverData)
        {
            ParseMain(values, ref errors, isCoverData);
        }

        public void ParseToEntityExtra(List<string> values, ref List<ImportErrorColModel> errors, bool isCoverData)
        {
            ParseExtra(values, ref errors, isCoverData);
        }
    }

    public class ImportExtraColModel
    {
        public int RowIndex { get; set; } = 1;

        public int ColIndex { get; set; } = 1;
    }

    public class ImportResultModel
    {
        public string Title { get; set; } = "";

        public ImportResult Result { get; set; } = ImportResult.Failed;

        public IList<ImportErrorRowModel> ErrorRows { get; set; } = new List<ImportErrorRowModel>();

        public IList<string> HeaderRow { get; set; } = new List<string>();
    }

    public enum ImportResult
    {
        Successful,
        Failed,
    }

    public class ImportErrorRowModel
    {
        /// <summary>
        /// 行号
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 原值
        /// </summary>
        public List<string> Values { get; set; } = new List<string>();

        /// <summary>
        /// 出错列集合
        /// </summary>
        public List<ImportErrorColModel> ErrorCols { get; set; } = new List<ImportErrorColModel>();
    }

    public class ImportErrorColModel
    {
        /// <summary>
        /// 出错列名
        /// </summary>
        public string ColName { get; set; }

        /// <summary>
        /// 出错列号
        /// </summary>
        public int ColIndex { get; set; } = 0;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }
    }

    #endregion

    #region MSDN
    public class Tag_ExportModel_Big
    {
        /// <summary>
        /// Excel里面的标签名称
        /// </summary>
        public string SheetName_Big { get; set; }
        public string SheetName_Small { get; set; }
        /// <summary>
        /// 导出文件全路径
        /// </summary>
        public string templateFilePath { get; set; }

        /// <summary>
        /// 危险化学品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 危险组分
        /// </summary>
        public string HS { get; set; }
        /// <summary>
        /// 危险品警示图地址集合
        /// </summary>
        public List<string> WarningPicPaths { get; set; }
        /// <summary>
        /// 站位空白图片
        /// </summary>
        public string BlankPicPath { get; set; }
        /// <summary>
        /// 警示标语
        /// </summary>
        public string WarningContent { get; set; }
        /// <summary>
        /// 危险简述
        /// </summary>
        public string HazardousDescription { get; set; }
        /// <summary>
        /// 预防措施
        /// </summary>
        public string DefenceDes { get; set;}
        /// <summary>
        /// 事故响应
        /// </summary>
        public string DealDES { get; set; }
        /// <summary>
        /// 安全存储
        /// </summary>
        public string StoreDes { get; set; }
        /// <summary>
        /// 废弃处置
        /// </summary>
        public string WasteHanding { get; set; }
        /// <summary>
        /// 供应商信息
        /// </summary>
        public string Supplier_Big { get; set; }
        public string Supplier_Small { get; set; }
    }

    public class Tag_ExportModel_Small
    {
        /// <summary>
        /// Excel里面的标签名称
        /// </summary>
        public string SheetName_Big { get; set; }
        public string SheetName_Small { get; set; }
        /// <summary>
        /// 导出文件全路径
        /// </summary>
        public string templateFilePath { get; set; }

        /// <summary>
        /// 危险化学品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 危险组分
        /// </summary>
        public string HS { get; set; }
        /// <summary>
        /// 危险品警示图地址集合
        /// </summary>
        public List<string> WarningPicPaths { get; set; }
        /// <summary>
        /// 站位空白图片
        /// </summary>
        public string BlankPicPath { get; set; }
        /// <summary>
        /// 警示标语
        /// </summary>
        public string WarningContent { get; set; }
        /// <summary>
        /// 危险简述
        /// </summary>
        public string HazardousDescription { get; set; }
        /// <summary>
        /// 预防措施
        /// </summary>
        public string DefenceDes { get; set; }
        /// <summary>
        /// 事故响应
        /// </summary>
        public string DealDES { get; set; }
        /// <summary>
        /// 安全存储
        /// </summary>
        public string StoreDes { get; set; }
        /// <summary>
        /// 废弃处置
        /// </summary>
        public string WasteHanding { get; set; }
        /// <summary>
        /// 供应商信息
        /// </summary>
        public string Supplier_Big { get; set; }
        public string Supplier_Small { get; set; }
    }

    public class Tag_ExportModel
    {
        /// <summary>
        /// Excel里面的标签名称
        /// </summary>
        public string SheetName_Big { get; set; }
        public string SheetName_Small { get; set; }
        /// <summary>
        /// 导出文件全路径
        /// </summary>
        public string templateFilePath { get; set; }
        /// <summary>
        /// 1:50*70 2:75*100 3:100*150 4:150*200 5:200*300
        /// </summary>
        public int Tag_Size { get; set; }

        /// <summary>
        /// 危险化学品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 危险组分
        /// </summary>
        public string HS { get; set; }
        /// <summary>
        /// 危险品警示图地址集合
        /// </summary>
        public List<string> WarningPicPaths { get; set; }
        /// <summary>
        /// 站位空白图片
        /// </summary>
        public string BlankPicPath { get; set; }
        /// <summary>
        /// 警示标语
        /// </summary>
        public string WarningContent { get; set; }
        /// <summary>
        /// 危险简述
        /// </summary>
        public string HazardousDescription { get; set; }
        /// <summary>
        /// 预防措施
        /// </summary>
        public string DefenceDes { get; set; }
        /// <summary>
        /// 事故响应
        /// </summary>
        public string DealDES { get; set; }
        /// <summary>
        /// 安全存储
        /// </summary>
        public string StoreDes { get; set; }
        /// <summary>
        /// 废弃处置
        /// </summary>
        public string WasteHanding { get; set; }
        /// <summary>
        /// 供应商信息
        /// </summary>
        public string Supplier_Big { get; set; }
        public string Supplier_Small { get; set; }
    }

    public class Tag_SecurityNotification
    {
        public string SheetName { get; set; }
        public string templateFilePath { get; set; }
        public string ProductName { get; set; }
        /// <summary>
        /// 危险品警示图地址集合
        /// </summary>
        public List<string> WarningPicPaths { get; set; }
        /// <summary>
        /// 个人防护图片集
        /// </summary>
        public List<string> ProtectionPicPaths { get; set; }
        /// <summary>
        /// 站位空白图片
        /// </summary>
        public string BlankPicPath { get; set; }
        /// <summary>
        /// 警示标语
        /// </summary>
        public string WarningContent { get; set; }
        /// <summary>
        /// 危险简述
        /// </summary>
        public string HazardousDescription { get; set; }
        /// <summary>
        /// 眼面部沾染
        /// </summary>
        public string Product_ET_FaceAndEye { get; set; }
        /// <summary>
        /// 皮肤沾染
        /// </summary>
        public string Product_ET_SkinAndHand { get; set; }
        /// <summary>
        /// 吸入
        /// </summary>
        public string Product_ET_Inhalation { get; set; }
        /// <summary>
        /// 食入
        /// </summary>
        public string Product_ET_Ingestion { get; set; }

    }
    #endregion

}
