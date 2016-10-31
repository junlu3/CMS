using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class MSDS_Substance_ExposureLimit
    {
        public Guid Id { get; set; }
        public string Substance_Name { get; set; }
        public string Substance_CN_Name { get; set; }
        public string CASCode { get; set; }

        #region NZ
        public string WorkSafe_NZ_TWA_PPM { get; set; }
        public string WorkSafe_NZ_TWA_MG { get; set; }
        public string WorkSafe_NZ_STEL_PPM { get; set; }
        public string WorkSafe_NZ_STEL_MG { get; set; }
        #endregion

        #region AUS
        public string WorkSafe_AUS_TWA_PPM { get; set; }
        public string WorkSafe_AUS_TWA_MG { get; set; }
        public string WorkSafe_AUS_STEL_PPM { get; set; }
        public string WorkSafe_AUS_STEL_MG { get; set; }
        #endregion

        #region GBZ 2.1 2007
        public string GBZ_TWA { get; set; }
        public string GBZ_STEL { get; set; }
        public string GBZ_MAC { get; set; }
        #endregion

        #region ACGIH TLV
        public string ACGIH_TLV_TWA_PPM { get; set; }
        public string ACGIH_TLV_TWA_MG { get; set; }
        public string ACGIH_TLV_STEL_PPM { get; set; }
        public string ACGIH_TLV_STEL_MG { get; set; }
        #endregion

        #region HSE UK EH40
        public string HSE_UK_EH40_TWA_PPM { get; set; }
        public string HSE_UK_EH40_TWA_MG { get; set; }
        public string HSE_UK_EH40_STEL_PPM { get; set; }
        public string HSE_UK_EH40_STEL_MG { get; set; }
        #endregion

        /// <summary>
        /// 分子量
        /// </summary>
        public string Molecular_Weight { get; set; }

        #region ERPG
        public string ERPG1 { get; set; }
        public string ERPG2 { get; set; }
        public string ERPG3 { get; set; }
        #endregion

        #region AEGL
        public string AEGL1_60 { get; set; }
        public string AEGL2_60 { get; set; }
        public string AEGL3_60 { get; set; }
        #endregion

        #region TEEL
        public string TEEL0 { get; set; }
        public string TEEL1 { get; set; }
        public string TEEL2 { get; set; }
        public string TEEL3 { get; set; }
        #endregion

        public string IDLH { get; set; }

        #region 致癌物分类
        /// <summary>
        /// 致癌物分类
        /// </summary>
        public string Cancerogen_IARC { get; set; }
        /// <summary>
        /// 致癌物分类
        /// </summary>
        public string Cancerogen_NTP { get; set; }
        /// <summary>
        /// 致癌物分类
        /// </summary>
        public string Cancerogen_ACGIH { get; set; }
        /// <summary>
        /// 致癌物分类
        /// </summary>
        public string Cancerogen_CP65 { get; set; }
        /// <summary>
        /// 致癌物分类
        /// </summary>
        public string Cancerogen_OSHA { get; set; }
        /// <summary>
        /// 致癌物分类
        /// </summary>
        public string Cancerogen_EPA { get; set; }
        #endregion

        #region 其他毒性
        /// <summary>
        /// 其他毒性-致畸
        /// </summary>
        public string Teratogenesis { get; set; }
        /// <summary>
        /// 其他毒性-生殖毒性
        /// </summary>
        public string Reproduction_Toxicity { get; set; }
        #endregion

        #region 应急反应
        /// <summary>
        /// 应急反应-北美
        /// </summary>
        public string ER_NA { get; set; }
        /// <summary>
        /// 应急反应-中国
        /// </summary>
        public string ER_CN { get; set; }
        #endregion

        /// <summary>
        /// 职业病危害因素目录
        /// </summary>
        public string Catalog { get; set; }
    }

    public class Substance_ExposureLimitSearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string Substance_Name { get; set; }
        public string Substance_CN_Name { get; set; }
        public string CASCode { get; set; }
    }
}
