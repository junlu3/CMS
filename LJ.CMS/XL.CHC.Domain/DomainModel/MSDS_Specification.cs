using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class MSDS_Specification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Product_Name { get; set; }
        public string CN_Name { get; set; }
        public string Purpose { get; set; }
        public int? Product_State { get; set; }
        public int? Product_UN { get; set; }
        public string CASCode { get; set; }
        public string Product_HazardousDescription { get; set; }
        public bool? UnHazardousChemical { get; set; }

        public string Supplier_Name { get; set; }
        public string Supplier_Address { get; set; }
        public string Supplier_Phone { get; set; }
        public string Supplier_UrgencyCall { get; set; }
        /// <summary>
        /// 附件路径
        /// </summary>
        public string AttachmentPath { get; set; }

        #region GHS
        public string GHS_Category { get; set; }
        public string GHS_Warning { get; set; }
        public string GHS_HazardouDes_Values { get; set; }
        public string GHS_HazardouDes_Append { get; set; }
        public string GHS_DefenceDes_Values { get; set; }
        public string GHS_DefenceDes_Append { get; set; }
        public string GHS_DealDES_Values { get; set; }
        public string GHS_DealDES_Append { get; set; }
        public string GHS_StoreDes_Values { get; set; }
        public string GHS_StoreDes_Append { get; set; }
        public bool IsExplosive { get; set; } = false;
        public bool IsFlammable { get; set; } = false;
        public bool IsCorrosive { get; set; } = false;
        public bool IsHealthHazard { get; set; } = false;
        public bool IsToxic { get; set; } = false;
        public bool IsOxidizing { get; set; } = false;
        public bool IsGasUnderPressure { get; set; } = false;
        public bool IsIrritant { get; set; } = false;
        public bool IsDangerousToEnvironment { get; set; } = false;
        #endregion

        #region 个人防护用品
        public string Product_Protection_FaceAndEye { get; set; }
        public string Product_Protection_Hand { get; set; }
        public string Product_Protection_Breathing { get; set; }
        public string Product_Protection_Foot { get; set; }
        public string Product_Protection_Body { get; set; }
        public string Product_Protection_Other { get; set; }

        public bool IsProtection_FaceAndEye { get; set; } = false;
        public bool IsProtection_Hand { get; set; } = false;
        public bool IsProtection_Breathing { get; set; } = false;
        public bool IsProtection_Foot { get; set; } = false;
        public bool IsProtection_Body { get; set; } = false;
        public bool IsProtection_Other { get; set; } = false;
        #endregion

        #region 急救措施  emergency treatment
        public string Product_ET_FaceAndEye { get; set; }
        public string Product_ET_SkinAndHand { get; set; }
        public string Product_ET_Inhalation { get; set; }
        public string Product_ET_Ingestion { get; set; }
        #endregion

        public string Product_FireProtection { get; set; }

        public string Product_LeakageHanding { get; set; }

        public string Product_OperationSecure { get; set; }

        public string Product_StoreRequirement { get; set; }

        public string Product_WasteHanding { get; set; }

        public string Product_Note { get; set; }

        #region 运输信息
        /// <summary>
        /// 当地法规
        /// </summary>
        public string Policie_Local { get; set; }
        /// <summary>
        /// 联合国法规
        /// </summary>
        public string Policie_UN { get; set; } = "";
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
        public decimal? BoilingPoint_Liquid { get; set; }
        /// <summary>
        /// 闪点 （单位： ℃）
        /// </summary>
        public decimal? FlashingPoint { get; set; }
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
        public decimal? VaporPressure { get; set; }
        public decimal? LD50_KG { get; set; }
        public decimal? LD50_L { get; set; }
        #endregion

        public DateTime? Create_Date { get; set; }
        public string Create_By { get; set; }
        public DateTime? Update_Date { get; set; }
        public string Update_By { get; set; }

        public virtual List<MSDS_HazardousSubstances> HazardousSubstance { get; set; } = new List<MSDS_HazardousSubstances>();
        public virtual MSDS_SpecificationCheck SpecificationCheck { get; set; }
        public virtual Company Company { get; set; }
        public virtual IList<MSDS_WorkStation> WorkStations { get; set; }
       
    }

    public class SpecificationSearchModel
    {
        public Guid Company_Id { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public DateTime? STime { get; set; }
        public DateTime? ETime { get; set; }
        public string Product_Name { get; set; }
        public string CN_Name { get; set; }
        public string Supplier_Name { get; set; }
        public string SortCol { get; set; }
        public string SortType { get; set; }
        public int? Product_UN { get; set; }
        public string CASCode { get; set; }
        public string HS_CASCode { get; set; }
        /// <summary>
        /// 审批状态 0 是未审批 1 是待审批 2 已审批 9 是显示全部
        /// </summary>
        public int? CheckStatus { get; set; }


    }
}
