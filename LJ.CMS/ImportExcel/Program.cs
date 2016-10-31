using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImportExcel
{
    class Program
    {
        static void Main(string[] args)
        {

           FileInfo file =  new FileInfo(@"C:\\暴露限值.xlsx");
            using (var xlPackage = new ExcelPackage(file))
            {
                List<Substance_ExposureLimit> list = new List<Substance_ExposureLimit>();
                for (char i = 'A'; i <= 'Z'; i++)
                {
                    var worksheet = xlPackage.Workbook.Worksheets[i.ToString()];
                     
                    for (int rowIndex = 1; rowIndex <= worksheet.Cells.Rows; rowIndex++)
                    {
                        if (rowIndex >= 4)
                        {
                            #region
                            Substance_ExposureLimit item = new Substance_ExposureLimit();

                            item.Substance_Name = worksheet.Cells[rowIndex, 1].Value?.ToString();
                            if (string.IsNullOrEmpty(item.Substance_Name))
                            {
                                continue;
                            }
                            item.Substance_CN_Name = worksheet.Cells[rowIndex, 2].Value?.ToString();
                            item.CASCode = worksheet.Cells[rowIndex, 3].Value?.ToString();

                            item.WorkSafe_NZ_TWA_PPM = worksheet.Cells[rowIndex, 4].Value?.ToString();
                            item.WorkSafe_NZ_TWA_MG = worksheet.Cells[rowIndex, 5].Value?.ToString();
                            item.WorkSafe_NZ_STEL_PPM = worksheet.Cells[rowIndex, 6].Value?.ToString();
                            item.WorkSafe_NZ_STEL_MG = worksheet.Cells[rowIndex, 7].Value?.ToString();

                            item.WorkSafe_AUS_TWA_PPM = worksheet.Cells[rowIndex, 8].Value?.ToString();
                            item.WorkSafe_AUS_TWA_MG = worksheet.Cells[rowIndex, 9].Value?.ToString();
                            item.WorkSafe_AUS_STEL_PPM = worksheet.Cells[rowIndex, 10].Value?.ToString();
                            item.WorkSafe_AUS_STEL_MG = worksheet.Cells[rowIndex, 11].Value?.ToString();

                            item.GBZ_TWA = worksheet.Cells[rowIndex, 12].Value?.ToString();
                            item.GBZ_STEL = worksheet.Cells[rowIndex, 13].Value?.ToString();
                            item.GBZ_MAC = worksheet.Cells[rowIndex, 14].Value?.ToString();
                            item.ACGIH_TLV_TWA_PPM = worksheet.Cells[rowIndex, 15].Value?.ToString();
                            item.ACGIH_TLV_TWA_MG = worksheet.Cells[rowIndex, 16].Value?.ToString();
                            item.ACGIH_TLV_STEL_PPM = worksheet.Cells[rowIndex, 17].Value?.ToString();
                            item.ACGIH_TLV_STEL_MG = worksheet.Cells[rowIndex, 18].Value?.ToString();

                            item.HSE_UK_EH40_TWA_PPM = worksheet.Cells[rowIndex, 19].Value?.ToString();
                            item.HSE_UK_EH40_TWA_MG = worksheet.Cells[rowIndex, 20].Value?.ToString();
                            item.HSE_UK_EH40_STEL_PPM = worksheet.Cells[rowIndex, 21].Value?.ToString();
                            item.HSE_UK_EH40_STEL_MG = worksheet.Cells[rowIndex, 22].Value?.ToString();

                            item.Molecular_Weight = worksheet.Cells[rowIndex, 23].Value?.ToString();

                            item.ERPG1 = worksheet.Cells[rowIndex, 24].Value?.ToString();
                            item.ERPG2 = worksheet.Cells[rowIndex, 25].Value?.ToString();
                            item.ERPG3 = worksheet.Cells[rowIndex, 26].Value?.ToString();

                            item.AEGL1_60 = worksheet.Cells[rowIndex, 27].Value?.ToString();
                            item.AEGL2_60 = worksheet.Cells[rowIndex, 28].Value?.ToString();
                            item.AEGL3_60 = worksheet.Cells[rowIndex, 29].Value?.ToString();

                            item.TEEL0 = worksheet.Cells[rowIndex, 30].Value?.ToString();
                            item.TEEL1 = worksheet.Cells[rowIndex, 31].Value?.ToString();
                            item.TEEL2 = worksheet.Cells[rowIndex, 32].Value?.ToString();
                            item.TEEL3 = worksheet.Cells[rowIndex, 33].Value?.ToString();

                            item.IDLH = worksheet.Cells[rowIndex, 34].Value?.ToString();

                            item.Cancerogen_IARC = worksheet.Cells[rowIndex, 35].Value?.ToString();
                            item.Cancerogen_NTP = worksheet.Cells[rowIndex, 36].Value?.ToString();
                            item.Cancerogen_ACGIH = worksheet.Cells[rowIndex, 37].Value?.ToString();
                            item.Cancerogen_CP65 = worksheet.Cells[rowIndex, 38].Value?.ToString();
                            item.Cancerogen_OSHA = worksheet.Cells[rowIndex, 39].Value?.ToString();
                            item.Cancerogen_EPA = worksheet.Cells[rowIndex, 40].Value?.ToString();

                            item.Teratogenesis = worksheet.Cells[rowIndex, 41].Value?.ToString();
                            item.Reproduction_Toxicity = worksheet.Cells[rowIndex, 42].Value?.ToString();

                            item.ER_NA = worksheet.Cells[rowIndex, 43].Value?.ToString();
                            item.ER_CN = worksheet.Cells[rowIndex, 44].Value?.ToString();

                            item.Catalog = worksheet.Cells[rowIndex, 45].Value?.ToString();

                            list.Add(item);
                            #endregion
                        }
                        
                    }
                   
                }
            }
            Console.ReadKey();
        }
    }

    public class Substance_ExposureLimit
    {
        public string Substance_Name { get; set; }
        public string Substance_CN_Name { get; set; }
        public string CASCode { get; set; }

        public string WorkSafe_NZ_TWA_PPM { get; set; }
        public string WorkSafe_NZ_TWA_MG { get; set; }
        public string WorkSafe_NZ_STEL_PPM { get; set; }
        public string WorkSafe_NZ_STEL_MG { get; set; }

        public string WorkSafe_AUS_TWA_PPM { get; set; }
        public string WorkSafe_AUS_TWA_MG { get; set; }
        public string WorkSafe_AUS_STEL_PPM { get; set; }
        public string WorkSafe_AUS_STEL_MG { get; set; }

        public string GBZ_TWA { get; set; }
        public string GBZ_STEL { get; set; }
        public string GBZ_MAC { get; set; }
        public string ACGIH_TLV_TWA_PPM { get; set; }
        public string ACGIH_TLV_TWA_MG { get; set; }
        public string ACGIH_TLV_STEL_PPM { get; set; }
        public string ACGIH_TLV_STEL_MG { get; set; }

        public string HSE_UK_EH40_TWA_PPM { get; set; }
        public string HSE_UK_EH40_TWA_MG { get; set; }
        public string HSE_UK_EH40_STEL_PPM { get; set; }
        public string HSE_UK_EH40_STEL_MG { get; set; }
        /// <summary>
        /// 分子量
        /// </summary>
        public string Molecular_Weight { get; set; }

        public string ERPG1 { get; set; }
        public string ERPG2 { get; set; }
        public string ERPG3 { get; set; }

        public string AEGL1_60 { get; set; }
        public string AEGL2_60 { get; set; }
        public string AEGL3_60 { get; set; }

        public string TEEL0 { get; set; }
        public string TEEL1 { get; set; }
        public string TEEL2 { get; set; }
        public string TEEL3 { get; set; }

        public string IDLH { get; set; }
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
        /// <summary>
        /// 其他毒性-致畸
        /// </summary>
        public string Teratogenesis { get; set; }
        /// <summary>
        /// 其他毒性-生殖毒性
        /// </summary>
        public string Reproduction_Toxicity { get; set; }
        /// <summary>
        /// 应急反应-北美
        /// </summary>
        public string ER_NA { get; set; }
        /// <summary>
        /// 应急反应-中国
        /// </summary>
        public string ER_CN { get; set; }
        /// <summary>
        /// 职业病危害因素目录
        /// </summary>
        public string Catalog { get; set; }
    }
}
