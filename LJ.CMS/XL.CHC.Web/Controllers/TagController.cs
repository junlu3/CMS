using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Web.Models;

namespace XL.CHC.Web.Controllers
{
    public class TagController : BaseController
    {
        #region Fields
        private readonly IMSDS_SpecificationService _specificationService;
        private readonly IMSDS_HazardousSubstancesService _hazardousSubstancesService;
        private readonly IMSDS_H_StatementService _h_StatementService;
        private readonly IMSDS_P_StatementService _p_StatementService;
        private readonly IImportExportService _importExportService;
        #endregion

        #region Ctor
        public TagController(IMSDS_SpecificationService specificationService, IMSDS_HazardousSubstancesService hazardousSubstancesService, IMSDS_H_StatementService h_StatementService, IMSDS_P_StatementService p_StatementService, IImportExportService importExportService)
        {
            _specificationService = specificationService;
            _hazardousSubstancesService = hazardousSubstancesService;
            _h_StatementService = h_StatementService;
            _p_StatementService = p_StatementService;
            _importExportService = importExportService;
        }
        #endregion

        #region Manage

        // GET: Tag
        public ActionResult Index()
        {
            try
            {
                var model = new SpecificationManageViewModel();
                SearchOrders(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Index(SpecificationManageViewModel searchModel)
        {
            try
            {
                SearchOrders(searchModel);
                return View(searchModel);
            }
            catch (Exception ex)
            {
                var model = new SpecificationManageViewModel();
                SearchOrders(model);
                ErrorNotification(ex);
                return View(model);
            }
        }

        public ActionResult Detail(Guid? id = null)
        {
            if (id != null)
            {

                Guid company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                var entity = _specificationService.Single(id.Value, company_Id);
                if (entity != null)
                {
                    var model = new SpecificationViewModel()
                    {
                        Id = entity.Id,
                        Product_Name = entity.Product_Name,
                        CN_Name = entity.CN_Name,
                        Purpose = entity.Purpose,
                        Product_State = entity.Product_State,
                        Product_UN = entity.Product_UN,
                        Product_HazardousDescription = entity.Product_HazardousDescription,
                        UnHazardousChemical = entity.UnHazardousChemical,
                        CASCode = entity.CASCode,
                        Supplier_Name = entity.Supplier_Name,
                        Supplier_Address = entity.Supplier_Address,
                        Supplier_Phone = entity.Supplier_Phone,
                        Supplier_UrgencyCall = entity.Supplier_UrgencyCall,
                        Product_HazardousSubstances = entity.HazardousSubstance,
                        AttachmentPath = entity.AttachmentPath,

                        GHS_Category = entity.GHS_Category,
                        GHS_Warning = entity.GHS_Warning,
                        GHS_HazardouDes_Append = entity.GHS_HazardouDes_Append,
                        GHS_DefenceDes_Append = entity.GHS_DefenceDes_Append,
                        GHS_DealDES_Append = entity.GHS_DealDES_Append,
                        GHS_StoreDes_Append = entity.GHS_StoreDes_Append,
                        IsExplosive = entity.IsExplosive,
                        IsFlammable = entity.IsFlammable,
                        IsCorrosive = entity.IsCorrosive,
                        IsHealthHazard = entity.IsHealthHazard,
                        IsToxic = entity.IsToxic,
                        IsOxidizing = entity.IsOxidizing,
                        IsGasUnderPressure = entity.IsGasUnderPressure,
                        IsIrritant = entity.IsIrritant,
                        IsDangerousToEnvironment = entity.IsDangerousToEnvironment,

                        Product_Protection_FaceAndEye = entity.Product_Protection_FaceAndEye,
                        Product_Protection_Hand = entity.Product_Protection_Hand,
                        Product_Protection_Breathing = entity.Product_Protection_Breathing,
                        Product_Protection_Foot = entity.Product_Protection_Foot,
                        Product_Protection_Body = entity.Product_Protection_Body,
                        Product_Protection_Other = entity.Product_Protection_Other,
                        IsProtection_FaceAndEye = entity.IsProtection_FaceAndEye,
                        IsProtection_Other = entity.IsProtection_Other,
                        IsProtection_Breathing = entity.IsProtection_Breathing,
                        IsProtection_Body = entity.IsProtection_Body,
                        IsProtection_Foot = entity.IsProtection_Foot,
                        IsProtection_Hand = entity.IsProtection_Hand,

                        Product_ET_FaceAndEye = entity.Product_ET_FaceAndEye,
                        Product_ET_SkinAndHand = entity.Product_ET_SkinAndHand,
                        Product_ET_Inhalation = entity.Product_ET_Inhalation,
                        Product_ET_Ingestion = entity.Product_ET_Ingestion,

                        Product_FireProtection = entity.Product_FireProtection,
                        Product_LeakageHanding = entity.Product_LeakageHanding,
                        Product_OperationSecure = entity.Product_OperationSecure,
                        Product_StoreRequirement = entity.Product_StoreRequirement,
                        Product_WasteHanding = entity.Product_WasteHanding,
                        Product_Note = entity.Product_Note,

                        Policie_UN = entity.Policie_UN,
                        Policie_Local = entity.Policie_Local,
                        ExposedLimit_Solid = entity.ExposedLimit_Solid,
                        ExposedLimit_Gas = entity.ExposedLimit_Gas,
                        PhysicalState = entity.PhysicalState,
                        AppearanceAndSmell = entity.AppearanceAndSmell,
                        BoilingPoint_Liquid_C = entity.BoilingPoint_Liquid.HasValue ? (double?)entity.BoilingPoint_Liquid.Value : null,
                        BoilingPoint_Liquid_F = ((entity.BoilingPoint_Liquid.HasValue ? (double?)(entity.BoilingPoint_Liquid.Value * 1.8M + 32) : null)),
                        FlashingPoint_C = entity.FlashingPoint.HasValue ? (double?)entity.FlashingPoint.Value : null,
                        FlashingPoint_F = ((entity.FlashingPoint.HasValue ? (double?)(entity.FlashingPoint.Value * 1.8M + 32) : null)),
                        BurningLimit_Max = entity.BurningLimit_Max,
                        BurningLimit_Min = entity.BurningLimit_Min,
                        VaporPressure = entity.VaporPressure.HasValue ? (double?)entity.VaporPressure.Value : null,
                        VaporPressure_Pa = (entity.VaporPressure.HasValue ? (double?)(entity.VaporPressure.Value * 133.32M) : null),
                        LD50_KG = entity.LD50_KG,
                        LD50_L = entity.LD50_L
                    };
                    PrepareSpecificationViewModel(model, entity);
                    return View(model);
                }
                else
                {
                    ErrorNotification(new Exception("编辑失败，未找到Id为" + id.ToString() + "的化学品"));
                    return RedirectToAction("Index");
                }
            }
            else
            {
                SpecificationViewModel model = new SpecificationViewModel();
                PrepareSpecificationViewModel(model, null);
                return View(model);
            }
        }

        public ActionResult HSDetail(string guid)
        {
            if (!string.IsNullOrEmpty(guid))
            {
                MSDS_HazardousSubstances hs = _hazardousSubstancesService.Single(Guid.Parse(guid));
                HazardousSubstancesViewModel viewModel = new HazardousSubstancesViewModel();
                viewModel.HS_Name = hs.HS_Name;
                viewModel.HS_MinPercent = hs.HS_MinPercent;
                viewModel.HS_MaxPercent = hs.HS_MaxPercent;
                viewModel.HS_CASCode = hs.HS_CASCode;
                viewModel.Id = hs.HS_Id;

                PrepareAddHazardousSubstancesViewModel(viewModel, hs);
                return PartialView(viewModel);
            }
            else
            {
                HazardousSubstancesViewModel viewModel = new HazardousSubstancesViewModel();
                PrepareAddHazardousSubstancesViewModel(viewModel, null);
                return PartialView(viewModel);
            }
        }

        public ActionResult Download(string id, int size)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    Guid company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                    MSDS_Specification entity = _specificationService.Single(Guid.Parse(id), company_Id);

                    string fileName = (entity.CN_Name ?? entity.Product_Name) + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
                    string filePath = Server.MapPath("~/Content/ExportFiles/" + fileName);
                    string templatePath = "~/Content/Templates/";
                    switch (size)
                    {
                        case 1:
                            templatePath += "GHS模板50-70.xlsx";
                            break;
                        case 2:
                            templatePath += "GHS模板75-100.xlsx";
                            break;
                        case 3:
                            templatePath += "GHS模板100-150.xlsx";
                            break;
                        case 4:
                            templatePath += "GHS模板150-200.xlsx";
                            break;
                        case 5:
                            templatePath += "GHS模板200-300.xlsx";
                            break;
                        default:
                            throw new Exception("未能找到对应的标签模板！");
                    }

                    System.IO.File.Copy(Server.MapPath(templatePath), filePath);


                    if (entity != null)
                    {
                        ExportToTag(filePath, entity);

                        return File(filePath, "text/xls", fileName);
                    }
                    else
                    {
                        ErrorNotification(new Exception("未能查询到该化学品信息！"));
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    ErrorNotification(ex);
                    throw ex;
                }
            }
        }

        public ActionResult Download2(string id)
        {
            try
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    Guid company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                    MSDS_Specification entity = _specificationService.Single(Guid.Parse(id), company_Id);

                    string fileName = "安全告知" + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
                    string filePath = Server.MapPath("~/Content/ExportFiles/" + fileName);
                    string templatePath = "~/Content/Templates/安全告知模板.xlsx";
                    string downloadName = "安全告知.xlsx";
                    System.IO.File.Copy(Server.MapPath(templatePath), filePath);


                    if (entity != null)
                    {
                        ExportToTag_SecurityNotification(filePath, entity);

                        return File(filePath, "text/xls", downloadName);
                    }
                    else
                    {
                        ErrorNotification(new Exception("未能查询到该化学品信息！"));
                        return View();
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                throw ex;
            }
        }
        #endregion

        #region  Utilities
        private void SearchOrders(SpecificationManageViewModel model)
        {
            try
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    var searchModel = new SpecificationSearchModel
                    {
                        PageIndex = model.PageIndex,
                        STime = model.STime,
                        ETime = model.ETime,
                        Product_Name = model.Product_Name,
                        CN_Name = model.CN_Name,
                        Supplier_Name = model.Supplier_Name,
                        HS_CASCode = model.HS_CASCode,
                        Product_UN = model.Product_UN,
                        CASCode = model.CASCode,
                        PageSize = model.PageSize,
                        SortCol = model.SortCol ?? "Product_Name",
                        SortType = model.SortType,
                        Company_Id = WorkContext.CurrentMembershipUser.Company.Id,
                        CheckStatus = 2
                    };
                    model.ViewList = _specificationService.Search(searchModel);
                }

                model.Company_Name = WorkContext.CurrentMembershipUser.Company.CompanyName;

            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
            }
        }

        private void PrepareSpecificationViewModel(SpecificationViewModel model, MSDS_Specification entity)
        {
            #region 加载状态DropDownList
            List<Product_State> stateList = new List<Product_State>();
            stateList.Add(new Product_State { State_Id = 0, State_Name = "" });
            stateList.Add(new Product_State { State_Id = 1, State_Name = "气态" });
            stateList.Add(new Product_State { State_Id = 2, State_Name = "液态" });
            stateList.Add(new Product_State { State_Id = 3, State_Name = "固态" });
            stateList.Add(new Product_State { State_Id = 4, State_Name = "气溶胶" });
            stateList.Add(new Product_State { State_Id = 5, State_Name = "凝胶" });
            stateList.Add(new Product_State { State_Id = 6, State_Name = "膏状物" });
            stateList.Add(new Product_State { State_Id = 7, State_Name = "其他" });

            List<string> warningSign = new List<string>();
            warningSign.Add("警 告");
            warningSign.Add("危 险");

            IList<MSDS_H_Statement> statments = _h_StatementService.GetAll();
            IList<MSDS_P_Statement> p_statments = _p_StatementService.GetAll();

            foreach (var item in stateList)
            {
                model.ProductStateItems.Add(new SelectListItem { Text = item.State_Name, Value = item.State_Id.ToString(), Selected = entity != null && item.State_Id == entity.Product_State });
            }

            foreach (var item in warningSign)
            {
                model.ProductWarningSignItems.Add(new SelectListItem { Text = item, Value = item, Selected = entity != null && item == entity.GHS_Warning });
            }
            #endregion

            if (entity != null && entity.UnHazardousChemical.HasValue && !entity.UnHazardousChemical.Value)
            {
                model.UnHazardousChemicalItems.Add(new SelectListItem { Text = "是", Value = "true", Selected = false });
                model.UnHazardousChemicalItems.Add(new SelectListItem { Text = "否", Value = "false", Selected = true });
            }
            else
            {
                model.UnHazardousChemicalItems.Add(new SelectListItem { Text = "是", Value = "true", Selected = true });
                model.UnHazardousChemicalItems.Add(new SelectListItem { Text = "否", Value = "false", Selected = false });
            }

            if (entity != null)
            {
                model.Id = entity.Id;
                char[] sp = new char[1] { ',' };
                #region
                foreach (MSDS_H_Statement item in statments)
                {
                    string[] tempArray = entity.GHS_HazardouDes_Values?.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    if (tempArray != null && tempArray.Any(o => o == item.Code))
                    {
                        model.GHS_HazardouDes_String += item.Description + "\r\n";
                    }
                }

                foreach (MSDS_P_Statement item in p_statments)
                {
                    string[] tempArray1 = entity.GHS_DefenceDes_Values?.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    if (tempArray1 != null && tempArray1.Any(o => o == item.Code))
                    {
                        model.GHS_DefenceDes_String += item.Description + "\r\n";
                    }
                    string[] tempArray2 = entity.GHS_DealDES_Values?.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    if (tempArray2 != null && tempArray2.Any(o => o == item.Code))
                    {
                        model.GHS_DealDES_String += item.Description + "\r\n";
                    }
                    string[] tempArray3 = entity.GHS_StoreDes_Values?.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    if (tempArray3 != null && tempArray3.Any(o => o == item.Code))
                    {
                        model.GHS_StoreDes_String += item.Description + "\r\n";
                    }
                }
                #endregion
            }
        }

        private void PrepareAddHazardousSubstancesViewModel(HazardousSubstancesViewModel model, MSDS_HazardousSubstances entity)
        {
            IList<MSDS_H_Statement> statments = _h_StatementService.GetAll();

            if (entity != null)
            {
                foreach (MSDS_H_Statement item in statments)
                {
                    model.HS_HStatementSel.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = entity.HS_HStatements.Any(o => o.Code == item.Code) });
                }
            }
            else
            {
                foreach (MSDS_H_Statement item in statments)
                {
                    model.HS_HStatementSel.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = false });
                }
            }

        }

        private void ExportToTag(string filePath, MSDS_Specification model)
        {
            Tag_ExportModel tag = new Tag_ExportModel();
            tag.SheetName_Big = "大标签";
            tag.SheetName_Small = "小标签";
            tag.templateFilePath = filePath;
            tag.ProductName = model.CN_Name ?? model.Product_Name;

            #region 危险组分
            tag.HS = "组分：";
            if (model.HazardousSubstance != null && model.HazardousSubstance.Count > 0)
            {
                int i = 1;
                foreach (MSDS_HazardousSubstances hs in model.HazardousSubstance)
                {
                    if (i > 1)
                    {
                        tag.HS += "/n";
                    }
                    tag.HS += hs.HS_Name + hs.HS_MinPercent + "-" + hs.HS_MaxPercent + "%";
                    i++;
                }
            }
            else
            {
                tag.HS = "";
            }
            #endregion

            #region 危险图标
            List<string> imgPathList = new List<string>();
            string strRootPath = Server.MapPath("~/Content/Images/");
            if (model.IsExplosive)
            {
                imgPathList.Add(strRootPath + "Explosive.png");
            }
            if (model.IsFlammable)
            {
                imgPathList.Add(strRootPath + "Flammable.png");
            }
            if (model.IsCorrosive)
            {
                imgPathList.Add(strRootPath + "Corrosive.png");
            }
            if (model.IsHealthHazard)
            {
                imgPathList.Add(strRootPath + "HealthHazard.png");
            }
            if (model.IsToxic)
            {
                imgPathList.Add(strRootPath + "Toxic.png");
            }
            if (model.IsOxidizing)
            {
                imgPathList.Add(strRootPath + "Oxidizing.png");
            }
            if (model.IsGasUnderPressure)
            {
                imgPathList.Add(strRootPath + "GasUnderPressure.png");
            }
            if (model.IsIrritant)
            {
                imgPathList.Add(strRootPath + "Irritant.png");
            }
            if (model.IsDangerousToEnvironment)
            {
                imgPathList.Add(strRootPath + "DangerousToEnvironment.png");
            }
            tag.WarningPicPaths = imgPathList;
            tag.BlankPicPath = strRootPath + "blank.png";
            #endregion

            #region 危害警示
            tag.WarningContent = model.GHS_Warning;
            #endregion

            char[] sp = new char[1] { ',' };
            #region 危害简述
            tag.HazardousDescription = "";
            if (!string.IsNullOrEmpty(model.GHS_HazardouDes_Values))
            {
                string[] names = model.GHS_HazardouDes_Values.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                IList<MSDS_H_Statement> h_statments = _h_StatementService.GetListByNames(names);
                foreach (MSDS_H_Statement item in h_statments)
                {
                    tag.HazardousDescription += "" + item.Description + "，";
                }
                tag.HazardousDescription = tag.HazardousDescription.Substring(0, tag.HazardousDescription.Length - 1);
            }
            else
            {
                tag.HazardousDescription = model.GHS_HazardouDes_Append;
            }
            #endregion

            #region 预防措施
            tag.DefenceDes = "";
            if (!string.IsNullOrEmpty(model.GHS_DefenceDes_Values))
            {
                string[] names = model.GHS_DefenceDes_Values.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                IList<MSDS_P_Statement> statements = _p_StatementService.GetListByNames(names);
                int i = 1;
                foreach (MSDS_P_Statement item in statements)
                {
                    tag.DefenceDes += "● " + item.Description + "\r\n";
                    i++;
                    if (i > 3) break;
                }
            }
            else
            {
                tag.DefenceDes = model.GHS_DefenceDes_Append;
            }
            #endregion

            #region 事故响应
            tag.DealDES = "";
            if (!string.IsNullOrEmpty(model.GHS_DealDES_Values))
            {
                string[] names = model.GHS_DealDES_Values.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                IList<MSDS_P_Statement> statements = _p_StatementService.GetListByNames(names);
                int i = 1;
                foreach (MSDS_P_Statement item in statements)
                {
                    tag.DealDES += "● " + item.Description + "\r\n";
                    i++;
                    if (i > 7) break;
                }
            }
            else
            {
                tag.DealDES = model.GHS_DealDES_Append;
            }
            #endregion

            #region 安全储存
            //tag.StoreDes = "";
            //if (!string.IsNullOrEmpty(model.GHS_StoreDes_Values))
            //{
            //    string[] names = model.GHS_StoreDes_Values.Split(sp, StringSplitOptions.RemoveEmptyEntries);
            //    IList<MSDS_P_Statement> statements = _p_StatementService.GetListByNames(names);
            //    int i = 1;
            //    foreach (MSDS_P_Statement item in statements)
            //    {
            //        tag.StoreDes += item.Description + ";";
            //        i++;
            //        if (i > 3) break;
            //    }
            //}
            //else
            //{
            //    tag.StoreDes = model.GHS_StoreDes_Append;
            //}

            //if (tag.StoreDes?.Length > 34)
            //{
            //    tag.StoreDes = tag.StoreDes.Substring(0, 34);
            //}
            tag.StoreDes = "存储化学品必须遵照国家法律、法规和其他有关的规定，化学品应有明显的标志。一般情况下，应避免阳光直射，保持通风良好，储存区域内严禁吸烟和使用明火。";
            #endregion

            #region 废弃处置
            //if (model.Product_WasteHanding?.Length > 95)
            //{
            //    tag.WasteHanding = model.Product_WasteHanding.Substring(0,95);
            //}
            //else
            //{
            //    tag.WasteHanding = model.Product_WasteHanding;
            //}
            tag.WasteHanding = "请遵从当地环保法规要求";
            #endregion

            #region 供应商信息
            tag.Supplier_Big = string.Format(@"供应商：{0}
地址：{1}
电话：{2}
                                 应急电话：{3}",
                            model.Supplier_Name,
                            model.Supplier_Address,
                            model.Supplier_Phone,
                            model.Supplier_UrgencyCall);

            tag.Supplier_Small = string.Format(@"供应商：{0}
地址：{1}
电话：{2}",
                            model.Supplier_Name,
                            model.Supplier_Address,
                            model.Supplier_Phone);
            #endregion

            _importExportService.ExportTagWithTemplate(tag);

        }

        private void ExportToTag_SecurityNotification(string filePath, MSDS_Specification model)
        {
            Tag_SecurityNotification tag = new Tag_SecurityNotification();
            tag.SheetName = "大标签";
            tag.templateFilePath = filePath;
            tag.ProductName = model.CN_Name ?? model.Product_Name;

            #region 危险图标
            List<string> imgPathList = new List<string>();
            string strRootPath = Server.MapPath("~/Content/Images/");
            if (model.IsExplosive)
            {
                imgPathList.Add(strRootPath + "Explosive.png");
            }
            if (model.IsFlammable)
            {
                imgPathList.Add(strRootPath + "Flammable.png");
            }
            if (model.IsCorrosive)
            {
                imgPathList.Add(strRootPath + "Corrosive.png");
            }
            if (model.IsHealthHazard)
            {
                imgPathList.Add(strRootPath + "HealthHazard.png");
            }
            if (model.IsToxic)
            {
                imgPathList.Add(strRootPath + "Toxic.png");
            }
            if (model.IsOxidizing)
            {
                imgPathList.Add(strRootPath + "Oxidizing.png");
            }
            if (model.IsGasUnderPressure)
            {
                imgPathList.Add(strRootPath + "GasUnderPressure.png");
            }
            if (model.IsIrritant)
            {
                imgPathList.Add(strRootPath + "Irritant.png");
            }
            if (model.IsDangerousToEnvironment)
            {
                imgPathList.Add(strRootPath + "DangerousToEnvironment.png");
            }
            tag.WarningPicPaths = imgPathList;
            tag.BlankPicPath = strRootPath + "blank.png";
            #endregion

            #region 个人防护图标
            List<string> imgPathList_Protection = new List<string>();
            if (model.IsProtection_FaceAndEye)
            {
                imgPathList_Protection.Add(strRootPath + "Product_Protection_FaceAndEye.png");
            }
            if (model.IsProtection_Hand)
            {
                imgPathList_Protection.Add(strRootPath + "Product_Protection_Hand.png");
            }
            if (model.IsProtection_Breathing)
            {
                imgPathList_Protection.Add(strRootPath + "Product_Protection_Breathing.png");
            }
            if (model.IsProtection_Foot)
            {
                imgPathList_Protection.Add(strRootPath + "Product_Protection_Foot.png");
            }
            if (model.IsProtection_Body)
            {
                imgPathList_Protection.Add(strRootPath + "Product_Protection_Body.png");
            }
            if (model.IsProtection_Other)
            {
                imgPathList_Protection.Add(strRootPath + "Product_Protection_Other.png");
            }
            tag.ProtectionPicPaths = imgPathList_Protection;
            #endregion

            char[] sp = new char[1] { ',' };
            #region 危害简述
            tag.HazardousDescription = "";
            if (!string.IsNullOrEmpty(model.GHS_HazardouDes_Values))
            {
                string[] names = model.GHS_HazardouDes_Values.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                IList<MSDS_H_Statement> h_statments = _h_StatementService.GetListByNames(names);
                int i = 1;
                foreach (MSDS_H_Statement item in h_statments)
                {
                    tag.HazardousDescription += "● " + item.Description + "\r\n";
                    i++;
                    if (i > 3) break;
                }
            }
            else
            {
                tag.HazardousDescription = model.GHS_HazardouDes_Append;
            }
            #endregion

            tag.Product_ET_FaceAndEye = model.Product_ET_FaceAndEye?.Replace("\r\n","");
            tag.Product_ET_SkinAndHand = model.Product_ET_SkinAndHand?.Replace("\r\n", "");
            tag.Product_ET_Inhalation = model.Product_ET_Inhalation?.Replace("\r\n", "");
            tag.Product_ET_Ingestion = model.Product_ET_Ingestion?.Replace("\r\n", "");

            _importExportService.ExportTagWithTemplate(tag);
        }
        #endregion
    }
}