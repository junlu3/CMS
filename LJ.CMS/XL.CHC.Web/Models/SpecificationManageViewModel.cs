using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Models
{
    /// <summary>
    /// 危险化学品化学品对象
    /// </summary>
    public class SpecificationViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage ="商品名称不能为空")]
        [MaxLength(200,ErrorMessage ="名称不能超过200个字")]
        public string Product_Name { get; set; }
        [MaxLength(200, ErrorMessage = "中文名不能超过200个字")]
        public string CN_Name { get; set; }
        [MaxLength(1000,ErrorMessage ="用途描述不能超过1000个字")]
        public string Purpose { get; set; }
        [RegularExpression(@"\d{1,}-\d{1,}-\d{1,}", ErrorMessage = "CAS 号格式错误。 e.g. 00-00-00 ")]
        public string CASCode { get; set; }

        public string AttachmentPath { get; set; }

        #region 产品基本信息
        [MaxLength(100, ErrorMessage = "用途描述不能超过100个字")]
        public string Supplier_Name { get; set; }
        public string Supplier_Address { get; set; }
        public string Supplier_Phone { get; set; }
        public string Supplier_UrgencyCall { get; set; }
        /// <summary>
        /// 1 气体, 2 液体 , 3 粉末
        /// </summary>
        public int? Product_State { get; set; }
        /// <summary>
        /// 状态下拉
        /// </summary>
        public List<SelectListItem> ProductStateItems { get; set; } = new List<SelectListItem>();
        public List<MSDS_HazardousSubstances> Product_HazardousSubstances { get; set; } = new List<MSDS_HazardousSubstances>();

        /// <summary>
        /// 警示词
        /// </summary>
        public string GHS_Warning { get; set; }
        public List<SelectListItem> ProductWarningSignItems { get; set; } = new List<SelectListItem>();
        #endregion

        #region 危害简述
        public int? Product_UN { get; set; }
        public string Product_HazardousDescription { get; set; }
        #endregion

        #region GHS
        /// <summary>
        /// 危险性分类
        /// </summary>
        public string GHS_Category { get; set; }


        /// <summary>
        /// 危险说明(标签集)
        /// </summary>
        public List<SelectListItem> GHS_HazardouDes { get; set; } = new List<SelectListItem>();
        /// <summary>
        /// 危险说明 值数组
        /// </summary>
        public string[] GHS_HazardouDes_Array { get; set; }
        public string GHS_HazardouDes_String{ get; set; }
        /// <summary>
        ///危险说明(人工录入)
        /// </summary>
        public string GHS_HazardouDes_Append { get; set; }

        /// <summary>
        /// 防范说明(预防) 标签集
        /// </summary>
        public List<SelectListItem> GHS_DefenceDes { get; set; } = new List<SelectListItem>();
        public string[] GHS_DefenceDes_Array { get; set; }
        public string GHS_DefenceDes_String { get; set; }
        /// <summary>
        /// 防范说明(预防) 人工录入
        /// </summary>
        public string GHS_DefenceDes_Append { get; set; }


        /// <summary>
        /// 防范说明(响应) 标签集
        /// </summary>
        public List<SelectListItem> GHS_DealDES { get; set; } = new List<SelectListItem>();
        public string[] GHS_DealDES_Array { get; set; }
        public string GHS_DealDES_String { get; set; }
        /// <summary>
        /// 防范说明(响应) 人工录入
        /// </summary>
        public string GHS_DealDES_Append { get; set; }


        /// <summary>
        /// 防范说明(储存) 标签集
        /// </summary>
        public List<SelectListItem> GHS_StoreDes { get; set; } = new List<SelectListItem>();
        public string[] GHS_StoreDes_Array { get; set; }
        public string GHS_StoreDes_String { get; set; }
        /// <summary>
        /// 防范说明(储存) 人工录入
        /// </summary>
        public string GHS_StoreDes_Append { get; set; }


        /// <summary>
        /// 爆炸物
        /// </summary>
        public bool IsExplosive { get; set; } = false;
        /// <summary>
        /// 易燃物
        /// </summary>
        public bool IsFlammable { get; set; } = false;
        /// <summary>
        /// 腐蚀
        /// </summary>
        public bool IsCorrosive { get; set; } = false;
        /// <summary>
        /// 长期健康危害
        /// </summary>
        public bool IsHealthHazard { get; set; } = false;
        /// <summary>
        /// 剧毒
        /// </summary>
        public bool IsToxic { get; set; } = false;
        /// <summary>
        /// 氧化剂
        /// </summary>
        public bool IsOxidizing { get; set; } = false;
        /// <summary>
        /// 高压气体
        /// </summary>
        public bool IsGasUnderPressure { get; set; } = false;
        /// <summary>
        /// 刺激性
        /// </summary>
        public bool IsIrritant { get; set; } = false;
        /// <summary>
        /// 环境危害
        /// </summary>
        public bool IsDangerousToEnvironment { get; set; } = false;
        #endregion
        //非危险化学品
        public bool? UnHazardousChemical { get; set; } = true;
        public List<SelectListItem> UnHazardousChemicalItems { get; set; } = new List<SelectListItem>();

        #region 个人防护用品
        /// <summary>
        /// 眼面部
        /// </summary>
        public string Product_Protection_FaceAndEye { get; set; }
        /// <summary>
        /// 手部
        /// </summary>
        public string Product_Protection_Hand { get; set; }
        /// <summary>
        /// 呼吸保护
        /// </summary>
        public string Product_Protection_Breathing { get; set; }
        /// <summary>
        /// 足部保护
        /// </summary>
        public string Product_Protection_Foot { get; set; }
        /// <summary>
        /// 身体保护
        /// </summary>
        public string Product_Protection_Body { get; set; }
        /// <summary>
        /// 其他防护
        /// </summary>
        public string Product_Protection_Other { get; set; }
        /// <summary>
        /// 眼面部
        /// </summary>
        public bool IsProtection_FaceAndEye { get; set; } = false;
        /// <summary>
        /// 手部
        /// </summary>
        public bool IsProtection_Hand { get; set; } = false;
        /// <summary>
        /// 呼吸保护
        /// </summary>
        public bool IsProtection_Breathing { get; set; } = false;
        /// <summary>
        /// 足部保护
        /// </summary>
        public bool IsProtection_Foot { get; set; } = false;
        /// <summary>
        /// 身体保护
        /// </summary>
        public bool IsProtection_Body { get; set; } = false;
        /// <summary>
        /// 其他防护
        /// </summary>
        public bool IsProtection_Other { get; set; } = false;
        #endregion

        #region 急救措施  emergency treatment
        public string Product_ET_FaceAndEye { get; set; }
        public string Product_ET_SkinAndHand { get; set; }
        /// <summary>
        /// 吸入
        /// </summary>
        public string Product_ET_Inhalation { get; set; }
        /// <summary>
        /// 食入
        /// </summary>
        public string Product_ET_Ingestion { get; set; }
        #endregion
        /// <summary>
        /// 应急消防
        /// </summary>
        public string Product_FireProtection { get; set; }
        /// <summary>
        /// 泄漏处置
        /// </summary>
        public string Product_LeakageHanding { get; set; }
        /// <summary>
        /// 作业安全
        /// </summary>
        public string Product_OperationSecure { get; set; }
        /// <summary>
        /// 储存要求
        /// </summary>
        public string Product_StoreRequirement { get; set; } 
        /// <summary>
        /// 废弃物处置
        /// </summary>
        public string Product_WasteHanding { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Product_Note { get; set; }

        #region 运输信息
        /// <summary>
        /// 当地法规
        /// </summary>
        public string Policie_Local { get; set; }
        /// <summary>
        /// 联合国法规
        /// </summary>
        public string Policie_UN { get; set; } = string.Empty;
        #endregion

        #region 理化性质
        /// <summary>
        /// 暴露限值(固体)
        /// </summary>
        public decimal? ExposedLimit_Solid { get; set; }
        /// <summary>
        /// 暴露限值(蒸汽/气体)
        /// </summary>
        public decimal? ExposedLimit_Gas { get; set; }
        /// <summary>
        /// 物理状态
        /// </summary>
        public string PhysicalState { get; set; }
        /// <summary>
        /// 外观/气味
        /// </summary>
        public string AppearanceAndSmell { get; set; }
        /// <summary>
        /// 沸点 （液体）
        /// </summary>
        public double? BoilingPoint_Liquid_C { get; set; }
        public double? BoilingPoint_Liquid_F { get; set; }
        /// <summary>
        /// 闪点 （单位： ℃）
        /// </summary>
        public double? FlashingPoint_C { get; set; }
        public double? FlashingPoint_F { get; set; }
        /// <summary>
        /// 燃烧极限范围 下限
        /// </summary>
        public decimal? BurningLimit_Min { get; set; }
        /// <summary>
        /// 燃烧极限范围 上线
        /// </summary>
        public decimal? BurningLimit_Max { get; set; }
        /// <summary>
        /// 蒸汽压
        /// </summary>
        public double? VaporPressure { get; set; }
        public double? VaporPressure_Pa { get; set; }
        public decimal? LD50_KG { get; set; }
        public decimal? LD50_L { get; set; }
        #endregion

        public string Notification { get; set; }
        public string NotificationType { get; set; }
        public DateTime? Create_Date { get; set; }
        public string Create_By { get; set; }
        public DateTime? Update_Date { get; set; }
        public string Update_By { get; set; }
    }
    public class HazardousSubstancesViewModel
    {
        public Guid? Specification_Id { get; set; }
        public Guid Id { get; set; }
        [Required(ErrorMessage = "不能为空")]
        public string HS_Name { get; set; }
        [Required(ErrorMessage = "最小值：不能为空")]
        public string HS_MinPercent { get; set; }
        [Required(ErrorMessage = "最大值：不能为空")]
        public string HS_MaxPercent { get; set; }

        //[RegularExpression(@"\d{1,}-\d{1,}-\d{1,}", ErrorMessage = "CAS 号格式错误。 e.g. 00-00-00 ")]
        public string HS_CASCode { get; set; }
        public string[] HS_HStatement_Str { get; set; }
        public List<SelectListItem> HS_HStatementSel { get; set; } = new List<SelectListItem>();
        public string Notification { get; set; }
        public string NotificationType { get; set; }
        public List<SelectListItem> CASCodes { get; set; }
    }
    public class Product_State
    {
        public string State_Name { get; set;}
        public int? State_Id { get; set; }
    }
    public class SpecificationManageViewModel
    {
       
        public IPagedList<MSDS_Specification> ViewList { get; set; }
        public int PageIndex { get; set; } = 1;
        public DateTime? STime { get; set; }
        public DateTime? ETime { get; set; }
        public string Product_Name { get; set; }
        public string CN_Name { get; set; }
        public string Supplier_Name { get; set; }
        public int PageSize { get; set; } = 15;
        public string SortCol { get; set; } = "Product_Name";
        public string SortType { get; set; } = "DESC";
        public int? Product_UN { get; set; }
        [RegularExpression(@"\d{1,}-\d{1,}-\d{1,}", ErrorMessage = "CAS 号格式错误。 e.g. 00-00-00 ")]
        public string CASCode { get; set; }
        [RegularExpression(@"\d{1,}-\d{1,}-\d{1,}", ErrorMessage = "CAS 号格式错误。 e.g. 00-00-00 ")]
        public string HS_CASCode { get; set; }
        public int CheckStatus { get; set; }
        public List<SelectListItem> Companys { get; set; }
        public string Company_Id { get; set; }
        public bool IsCurrentCompany { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
        public string Company_Name { get; set; }
        public string ActionType { get; set; } = "Search";

    }
    public class SpecificationImportViewModel
    {
        public Guid? Id { get; set; }
    }
    public class SubstanceExposureLimitViewMode
    {
        public MSDS_Specification Specification { get; set; } = new MSDS_Specification();
        public IList<MSDS_Substance_ExposureLimit> ViewList { get; set; } = new List<MSDS_Substance_ExposureLimit>();
    }

}