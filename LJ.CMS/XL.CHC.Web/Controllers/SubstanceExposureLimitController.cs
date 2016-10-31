using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Web.Controllers
{
    public class SubstanceExposureLimitController : BaseController
    {
        #region Fields
        private readonly IMSDS_SpecificationService _specificationService;
        private readonly IMSDS_Substance_ExposureLimitService _substanceExposureLimitService;

        delegate Guid ProcessTask(Stream stream,Guid id);
        private static object syncRoot = new object();
        private static IDictionary<Guid, double> ProcessStatus { get; set; } = new Dictionary<Guid, double>();

        #endregion

        public SubstanceExposureLimitController(IMSDS_Substance_ExposureLimitService substanceExposureLimitService,
            IMSDS_SpecificationService specificationService)
        {
            _specificationService = specificationService;
            _substanceExposureLimitService = substanceExposureLimitService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Import()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                ErrorNotification(new Exception("上传失败，尝试上传无效的文件"));
                return View();
            }
        }
        [HttpPost]
        public ActionResult Import(object i)
        {
            try
            {
                var file = Request.Files["ImportExcelFile"];

                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType.Equals("text/xls") || file.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                    {
                        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                        {
                            char[] sp = new char[1] { '.' };
                            string[] strSuffix = file.FileName?.Split(sp);
                            if (strSuffix != null && strSuffix.Length == 2)
                            {

                                string fileName = "SE_" + DateTime.Now.Ticks.ToString() + "." + strSuffix[1];
                                string filePath = Server.MapPath("~/Content/Upload/" + fileName);
                                file.SaveAs(filePath);
                                var inputSteam = file.InputStream;

                                Guid id= StartAsyncProcess(inputSteam);
                                ViewBag.Id = id.ToString();
                                //entiy.AttachmentPath = "/Content/Upload/" + fileName;
                                SuccessNotification("上传成功！正在执行数据导入...");
                                return View();
                            }
                            else
                            {
                                ErrorNotification(new Exception("上传失败，上传无效的文件"));
                                return View();
                            }
                        }
                    }
                    else
                    {
                        ErrorNotification(new Exception("上传失败，请上传EXCEL文件"));
                        return View();
                    }
                }
                else
                {
                    ErrorNotification(new Exception("上传失败，上传无效的文件"));
                    return View();
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return View();
            }
        }

        public ActionResult GetStatus(Guid? id)
        {
            try
            {
                if (id != null)
                {
                    double status = GetImportStatus(id.Value);
                    return Content(status.ToString());
                }
                else
                {
                    return Content(Boolean.FalseString);
                }
            }
            catch (Exception)
            {

                return Content(Boolean.FalseString);
            }
        }

        private Guid Import(Stream stream,Guid id)
        {
            try
            { 
                IList<MSDS_Substance_ExposureLimit> list = ReadExcel(stream, id);

                if (list.Count>0)
                {
                    using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                    {
                        var allEntities = _substanceExposureLimitService.GetAll();

                        _substanceExposureLimitService.DeleteAll(allEntities);

                        _substanceExposureLimitService.Add(list);

                        unitOfWork.Commit();
                    }
                }

                return id;
            }
            catch (Exception)
            {
                lock (syncRoot)
                {
                    ProcessStatus[id] = -1;
                }
                Thread.Sleep(10000);
                return id;
            }


        }

        private IList<MSDS_Substance_ExposureLimit> ReadExcel(Stream stream,Guid id)
        {
            try
            {
                List<MSDS_Substance_ExposureLimit> list = new List<MSDS_Substance_ExposureLimit>();
                using (var xlPackage = new ExcelPackage(stream))
                {
                    int k = 1;
                    for (char i = 'A'; i <= 'Z'; i++)
                    {
                        var worksheet = xlPackage.Workbook.Worksheets[i.ToString()];
                        if (worksheet == null)
                        {
                            continue;
                        }
                        int totalAmount = worksheet.Cells.Rows;
                        for (int rowIndex = 1; rowIndex <= totalAmount; rowIndex++)
                        {
                            if (rowIndex >= 4)
                            {
                                #region
                                MSDS_Substance_ExposureLimit item = new MSDS_Substance_ExposureLimit();
                                item.Id = Guid.NewGuid();
                                item.Substance_Name = worksheet.Cells[rowIndex, 1].Value?.ToString();
                                item.Substance_CN_Name = worksheet.Cells[rowIndex, 2].Value?.ToString();
                                item.CASCode = worksheet.Cells[rowIndex, 3].Value?.ToString();
                                if (string.IsNullOrEmpty(item.Substance_Name) && string.IsNullOrEmpty(item.CASCode) && string.IsNullOrEmpty(item.Substance_CN_Name))
                                {
                                    break;
                                }
                                else if (string.IsNullOrEmpty(item.Substance_Name) || string.IsNullOrEmpty(item.CASCode))
                                {
                                    continue;
                                }
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
                        lock (syncRoot)
                        {
                            ProcessStatus[id] = (double)(k*100 / 26);
                        }
                        k++;
                    }
                    stream.Close();
                    stream.Dispose();
                }

                return list;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private Guid StartAsyncProcess(Stream inputSteam)
        {
            Guid id = Guid.NewGuid();
            Add(id);
            ProcessTask processTask = new ProcessTask(Import);
            processTask.BeginInvoke(inputSteam, id, new AsyncCallback(EndAsyncProcess), processTask);
            return id;
        }

        private void EndAsyncProcess(IAsyncResult result)
        {
            try
            {
                ProcessTask processTask = (ProcessTask)result.AsyncState;
                Guid id = processTask.EndInvoke(result);
                Remove(id);
            }
            catch (Exception)
            {
            }

        }

        private void Add(Guid id)
        {
            lock (syncRoot)
            {
                ProcessStatus.Add(id, 0);
            }
        }

        private void Remove(Guid id)
        {
            lock (syncRoot)
            {
                ProcessStatus.Remove(id);
            }
        }

        private double GetImportStatus(Guid id)
        {
            lock (syncRoot)
            {
                if (ProcessStatus.Keys.Any(x => x == id))
                {
                    return ProcessStatus[id];
                }
                else
                {
                    return 100;
                }
            }
        }

    }
}