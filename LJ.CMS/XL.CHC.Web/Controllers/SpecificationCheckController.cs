using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Web.Models;
using XL.Utilities;

namespace XL.CHC.Web.Controllers
{
    public class SpecificationCheckController : BaseController
    {

        #region Fields
        private readonly IMSDS_SpecificationService _specificationService;
        private readonly IMSDS_HazardousSubstancesService _hazardousSubstancesService;
        private readonly IMSDS_H_StatementService _h_StatementService;
        private readonly IMSDS_P_StatementService _p_StatementService;
        private readonly IImportExportService _importExportService;
        private readonly IMSDS_SpecificationCheckService _specificationCheckService;
        private readonly ICompanyService _companyService;
        private readonly IMSDS_CompositionService _compositionService;
        private readonly IEmailService _emailService;
        #endregion

        #region Ctor
        public SpecificationCheckController(IMSDS_SpecificationService specificationService, 
            IMSDS_HazardousSubstancesService hazardousSubstancesService, 
            IMSDS_H_StatementService h_StatementService, 
            IMSDS_P_StatementService p_StatementService, 
            IImportExportService importExportService,
            IMSDS_SpecificationCheckService specificationCheckService,
            ICompanyService companyService,
            IMSDS_CompositionService compositionService,
            IEmailService emailService)
        {
            _specificationService = specificationService;
            _hazardousSubstancesService = hazardousSubstancesService;
            _h_StatementService = h_StatementService;
            _p_StatementService = p_StatementService;
            _importExportService = importExportService;
            _specificationCheckService = specificationCheckService;
            _companyService = companyService;
            _compositionService = compositionService;
            _emailService = emailService;
        }
        #endregion

        // GET: SpecificationCheck
        public ActionResult Index()
        {
            try
            {
                MembershipUser user = WorkContext.CurrentMembershipUser;
                var model = new SpecificationManageViewModel();
                model.CheckStatus = 0;
                ViewBag.Company_Id = user.Company.Id;
                SearchOrders(model);
                return View(model);
            }
            catch (Exception ex)
            {
                var model = new SpecificationManageViewModel();
                model.CheckStatus = 0;
                ViewBag.Company_Id = Guid.Empty;
                ErrorNotification(ex);
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult Index(SpecificationManageViewModel searchModel)
        {
            try
            {
                searchModel.CheckStatus = 0;
                switch (searchModel.ActionType)
                {
                    case "Download":
                        return Download(searchModel);
                    case "Search":
                    default:
                        SearchOrders(searchModel);
                        return View(searchModel);

                }               
            }
            catch (Exception ex)
            {
                var model = new SpecificationManageViewModel();
                SearchOrders(model);
                ErrorNotification(ex);
                return View(model);
            }
        }

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
                        CheckStatus = model.CheckStatus
                        
                    };
                    model.ViewList = _specificationService.Search(searchModel);
                }

            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
            }
        }

        public ActionResult Download(SpecificationManageViewModel model)
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
                    PageSize = int.MaxValue,
                    SortCol = model.SortCol ?? "Product_Name",
                    SortType = model.SortType,
                    Company_Id = WorkContext.CurrentMembershipUser.Company.Id,
                    CheckStatus = model.CheckStatus

                };
                model.ViewList = _specificationService.Search(searchModel);

                try
                {
                    string suffix = DateTime.Now.ToString("yyyyMMddhhmmss");
                    var fileName = "危险化学品列表" + suffix + ".xlsx";
                    var filePath = Server.MapPath("~/Content/ExportFiles/" + fileName);
                    System.IO.File.Copy(Server.MapPath("~/Content/Templates/危险化学品目录模板(导出).xlsx"), filePath);

                    _specificationCheckService.ExportSpecificationResult(filePath, model.ViewList.ToList());

                    return File(filePath, "text/xls", fileName);
                }
                catch (Exception ex)
                {
                    ErrorNotification(ex);
                    return RedirectToAction("Search", new { @model = model });
                }
            }
        }

        #region Manage
        public ActionResult CreateOrUpdate(Guid? id = null)
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
                    ErrorNotification(new Exception("加载失败，未找到Id为" + id.ToString() + "的化学品"));
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

        public ActionResult BaseProductInfo(SpecificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    MSDS_Specification entity = null;
                    var company = _companyService.GetById(WorkContext.CurrentMembershipUser.Company.Id);
                    using (var uniOfWork = UnitOfWorkManager.NewUnitOfWork())
                    {
                        entity = new MSDS_Specification()
                        {
                            Id = Guid.NewGuid(),
                            Product_Name = model.Product_Name?.Trim(),
                            CN_Name = model.CN_Name?.Trim(),
                            Purpose = model.Purpose,
                            Product_State = model.Product_State,
                            Product_UN = model.Product_UN,
                            Product_HazardousDescription = model.Product_HazardousDescription,
                            Supplier_Name = model.Supplier_Name,
                            Supplier_Address = model.Supplier_Address,
                            Supplier_Phone = model.Supplier_Phone,
                            Supplier_UrgencyCall = model.Supplier_UrgencyCall,
                            UnHazardousChemical = model.UnHazardousChemical,
                            CASCode = model.CASCode?.Trim(),
                            Create_Date = DateTime.Now,
                            Create_By = WorkContext.CurrentMembershipUser.Username,
                            Company = company
                        };

                        _specificationService.Add(entity);
                        uniOfWork.Commit();
                    }

                    SuccessNotification("保存成功");
                    //model.Notification = "保存成功";
                    //model.NotificationType = "True";

                    //PrepareSpecificationViewModel(model, entity);
                    //model.Id = entity.Id;
                    //return View(model);
                    return RedirectToAction("CreateOrUpdate", new { id = entity.Id });
                }
                else
                {
                    Guid company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                    var entity = _specificationService.Single(model.Id, company_Id);
                    if (entity != null)
                    {
                        using (var unitOfWordk = UnitOfWorkManager.NewUnitOfWork())
                        {
                            entity.Product_Name = model.Product_Name?.Trim();
                            entity.CN_Name = model.CN_Name?.Trim();
                            entity.Purpose = model.Purpose;
                            entity.Product_State = model.Product_State;
                            entity.Product_UN = model.Product_UN;
                            entity.Product_HazardousDescription = model.Product_HazardousDescription;
                            entity.Supplier_Name = model.Supplier_Name;
                            entity.Supplier_Address = model.Supplier_Address;
                            entity.Supplier_Phone = model.Supplier_Phone;
                            entity.Supplier_UrgencyCall = model.Supplier_UrgencyCall;
                            entity.UnHazardousChemical = model.UnHazardousChemical;
                            entity.CASCode = model.CASCode?.Trim();
                            entity.Update_Date = DateTime.Now;
                            entity.Update_By = WorkContext.CurrentMembershipUser.Username;

                            unitOfWordk.Commit();

                            
                            //model.Notification = "保存成功";
                            //model.NotificationType = "True";
                            //PrepareSpecificationViewModel(model, entity);
                            
                        }
                        SuccessNotification("保存成功");
                        return RedirectToAction("CreateOrUpdate", new { id = entity.Id });
                    }
                    else
                    {

                        model.Notification = "保存失败，未找到Id为" + model.Id.ToString() + "的化学品";
                        model.NotificationType = "False";
                        PrepareSpecificationViewModel(model, null);
                        return View(model);
                    }

                }
            }
            else
            {

                model.Notification = "编辑失败，输入信息有误";
                model.NotificationType = "False";
                PrepareSpecificationViewModel(model, null);
                return View(model);
            }
        }

        public ActionResult UpdateGHS(SpecificationViewModel model)
        {

            if (model.Id.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                using (var unitOfWordk = UnitOfWorkManager.NewUnitOfWork())
                {
                    Guid company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                    MSDS_Specification specification = _specificationService.Single(model.Id, company_Id);

                    specification.GHS_HazardouDes_Append = model.GHS_HazardouDes_Append;

                    if (model.GHS_HazardouDes_Array != null)
                    {
                        string strArray = string.Empty;
                        foreach (string item in model.GHS_HazardouDes_Array)
                        {
                            strArray += item + ",";
                        }
                        strArray = strArray.Substring(0, strArray.Length - 1);
                        specification.GHS_HazardouDes_Values = strArray;

                    }
                    else
                    {
                        specification.GHS_HazardouDes_Values = "";
                    }

                    specification.GHS_DefenceDes_Append = model.GHS_DefenceDes_Append;
                    if (model.GHS_DefenceDes_Array != null)
                    {
                        string strArray = string.Empty;
                        foreach (string item in model.GHS_DefenceDes_Array)
                        {
                            strArray += item + ",";
                        }
                        strArray = strArray.Substring(0, strArray.Length - 1);
                        specification.GHS_DefenceDes_Values = strArray;

                    }
                    else
                    {
                        specification.GHS_DefenceDes_Values = "";
                    }

                    specification.GHS_DealDES_Append = model.GHS_DealDES_Append;
                    if (model.GHS_DealDES_Array != null)
                    {
                        string strArray = string.Empty;
                        foreach (string item in model.GHS_DealDES_Array)
                        {
                            strArray += item + ",";
                        }
                        strArray = strArray.Substring(0, strArray.Length - 1);
                        specification.GHS_DealDES_Values = strArray;

                    }
                    else
                    {
                        specification.GHS_DealDES_Values = "";
                    }

                    specification.GHS_StoreDes_Append = model.GHS_StoreDes_Append;
                    if (model.GHS_StoreDes_Array != null)
                    {
                        string strArray = string.Empty;
                        foreach (string item in model.GHS_StoreDes_Array)
                        {
                            strArray += item + ",";
                        }
                        strArray = strArray.Substring(0, strArray.Length - 1);
                        specification.GHS_StoreDes_Values = strArray;

                    }
                    else
                    {
                        specification.GHS_StoreDes_Values = "";
                    }

                    int i = 0;
                    if (specification.IsExplosive != model.IsExplosive)
                    {
                        specification.IsExplosive = model.IsExplosive;
                    }

                    if (specification.IsFlammable != model.IsFlammable)
                    {
                        specification.IsFlammable = model.IsFlammable;
                    }

                    if (specification.IsCorrosive != model.IsCorrosive)
                    {
                        specification.IsCorrosive = model.IsCorrosive;
                    }

                    if (specification.IsHealthHazard != model.IsHealthHazard)
                    {
                        specification.IsHealthHazard = model.IsHealthHazard;
                    }

                    if (specification.IsToxic != model.IsToxic)
                    {
                        specification.IsToxic = model.IsToxic;
                    }

                    if (specification.IsOxidizing != model.IsOxidizing)
                    {
                        specification.IsOxidizing = model.IsOxidizing;
                    }

                    if (specification.IsGasUnderPressure != model.IsGasUnderPressure)
                    {
                        specification.IsGasUnderPressure = model.IsGasUnderPressure;
                    }

                    if (specification.IsIrritant != model.IsIrritant)
                    {
                        specification.IsIrritant = model.IsIrritant;
                    }

                    if (specification.IsDangerousToEnvironment != model.IsDangerousToEnvironment)
                    {
                        specification.IsDangerousToEnvironment = model.IsDangerousToEnvironment;
                    }

                    if (specification.IsExplosive) i++;
                    if (specification.IsFlammable) i++;
                    if (specification.IsCorrosive) i++;
                    if (specification.IsHealthHazard) i++;
                    if (specification.IsToxic) i++;
                    if (specification.IsOxidizing) i++;
                    if (specification.IsGasUnderPressure) i++;
                    if (specification.IsIrritant) i++;
                    if (specification.IsDangerousToEnvironment) i++;

                    if (i > 5)
                    {
                        model.Notification = "保存失败！警示图标不能超过5个";
                        model.NotificationType = "False";
                        PrepareSpecificationViewModel(model, null);
                        return PartialView(model);
                    }
                    else
                    {
                        specification.GHS_Category = model.GHS_Category;
                        specification.GHS_Warning = model.GHS_Warning;
                        specification.Update_By = WorkContext.CurrentMembershipUser.Username;
                        specification.Update_Date = DateTime.Now;

                        unitOfWordk.Commit();

                        model.Notification = "保存成功";
                        model.NotificationType = "True";

                        PrepareSpecificationViewModel(model, specification);
                        return PartialView(model);
                    }

                }


            }
            else
            {
                model.Notification = "保存失败，输入信息有误";
                model.NotificationType = "False";
                PrepareSpecificationViewModel(model, null);
                return PartialView(model);
            }
        }

        public ActionResult Other(SpecificationViewModel model)
        {
            if (model.Id.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                using (var unitOfWordk = UnitOfWorkManager.NewUnitOfWork())
                {
                    Guid company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                    MSDS_Specification specification = _specificationService.Single(model.Id, company_Id);
                    if (specification.IsProtection_FaceAndEye != model.IsProtection_FaceAndEye)
                    {
                        specification.IsProtection_FaceAndEye = model.IsProtection_FaceAndEye;
                    }

                    if (specification.IsProtection_Hand != model.IsProtection_Hand)
                    {
                        specification.IsProtection_Hand = model.IsProtection_Hand;
                    }
                    if (specification.IsProtection_Breathing != model.IsProtection_Breathing)
                    {
                        specification.IsProtection_Breathing = model.IsProtection_Breathing;
                    }
                    if (specification.IsProtection_Foot != model.IsProtection_Foot)
                    {
                        specification.IsProtection_Foot = model.IsProtection_Foot;
                    }

                    if (specification.IsProtection_Body != model.IsProtection_Body)
                    {
                        specification.IsProtection_Body = model.IsProtection_Body;
                    }

                    if (specification.IsProtection_Other != model.IsProtection_Other)
                    {
                        specification.IsProtection_Other = model.IsProtection_Other;
                    }

                    specification.Product_Protection_FaceAndEye = model.Product_Protection_FaceAndEye;
                    specification.Product_Protection_Hand = model.Product_Protection_Hand;
                    specification.Product_Protection_Breathing = model.Product_Protection_Breathing;
                    specification.Product_Protection_Foot = model.Product_Protection_Foot;
                    specification.Product_Protection_Body = model.Product_Protection_Body;
                    specification.Product_Protection_Other = model.Product_Protection_Other;


                    specification.Product_ET_FaceAndEye = model.Product_ET_FaceAndEye;
                    specification.Product_ET_SkinAndHand = model.Product_ET_SkinAndHand;
                    specification.Product_ET_Inhalation = model.Product_ET_Inhalation;
                    specification.Product_ET_Ingestion = model.Product_ET_Ingestion;

                    specification.Product_FireProtection = model.Product_FireProtection;
                    specification.Product_LeakageHanding = model.Product_LeakageHanding;
                    specification.Product_OperationSecure = model.Product_OperationSecure;
                    specification.Product_StoreRequirement = model.Product_StoreRequirement;
                    specification.Product_WasteHanding = model.Product_WasteHanding;
                    specification.Product_Note = model.Product_Note;
                    specification.Policie_Local = model.Policie_Local;
                    specification.Policie_UN = model.Policie_UN;
                    specification.Update_Date = DateTime.Now;
                    specification.Update_By = WorkContext.CurrentMembershipUser.Username;

                    unitOfWordk.Commit();

                    model.Notification = "保存成功";
                    model.NotificationType = "True";
                    PrepareSpecificationViewModel(model, specification);
                }

                return View(model);
            }
            else
            {
                model.Notification = "保存失败，输入信息有误";
                model.NotificationType = "False";
                PrepareSpecificationViewModel(model, null);
                return View(model);
            }
        }

        public ActionResult PhysicoChemical(SpecificationViewModel model)
        {
            if (model.Id.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                using (var unitOfWordk = UnitOfWorkManager.NewUnitOfWork())
                {
                    Guid company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                    MSDS_Specification specification = _specificationService.Single(model.Id, company_Id);

                    specification.Policie_Local = model.Policie_Local;
                    specification.Policie_UN = model.Policie_UN;
                    specification.ExposedLimit_Solid = model.ExposedLimit_Solid;
                    specification.ExposedLimit_Gas = model.ExposedLimit_Gas;
                    specification.PhysicalState = model.PhysicalState;
                    specification.AppearanceAndSmell = model.AppearanceAndSmell;
                    specification.BoilingPoint_Liquid = (decimal?)model.BoilingPoint_Liquid_C;
                    specification.FlashingPoint = (decimal?)model.FlashingPoint_C;
                    specification.BurningLimit_Min = model.BurningLimit_Min;
                    specification.BurningLimit_Max = model.BurningLimit_Max;
                    specification.VaporPressure = (decimal?)model.VaporPressure;
                    specification.LD50_KG = model.LD50_KG;
                    specification.LD50_L = model.LD50_L;
                    specification.Update_Date = DateTime.Now;
                    specification.Update_By = WorkContext.CurrentMembershipUser.Username;

                    unitOfWordk.Commit();

                    model.Notification = "保存成功";
                    model.NotificationType = "True";
                    PrepareSpecificationViewModel(model, specification);
                }
                return View(model);
            }
            else
            {
                model.Notification = "保存失败，输入信息有误";
                model.NotificationType = "False";
                PrepareSpecificationViewModel(model, null);
                return View(model);
            }
        }

        public ActionResult AddHazardousSubstances(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                MSDS_HazardousSubstances hs = _hazardousSubstancesService.Single(Guid.Parse(name));
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


        [HttpPost]
        public ActionResult AddHazardousSubstances(HazardousSubstancesViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model != null && model.Specification_Id.Value.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    if (model.Specification_Id.ToString() != "00000000-0000-0000-0000-000000000000")
                    {
                        using (var uniOfWork = UnitOfWorkManager.NewUnitOfWork())
                        {
                            Guid company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                            MSDS_Specification entity = _specificationService.Single(model.Specification_Id.Value, company_Id);
                            MSDS_HazardousSubstances hs = entity.HazardousSubstance.SingleOrDefault<MSDS_HazardousSubstances>(o => o.HS_Id == model.Id);

                            if (hs != null)
                            {

                                hs.HS_Name = model.HS_Name?.Trim();
                                hs.HS_MinPercent = model.HS_MinPercent;
                                hs.HS_MaxPercent = model.HS_MaxPercent;
                                hs.HS_CASCode = model.HS_CASCode?.Trim();

                                if (model.HS_HStatement_Str != null)
                                {
                                    hs.HS_HStatements.Clear();
                                    hs.HS_HStatements = _h_StatementService.GetListByNames(model.HS_HStatement_Str);
                                }
                                else
                                {
                                    if (hs.HS_HStatements == null)
                                    {
                                        hs.HS_HStatements = new List<MSDS_H_Statement>();
                                    }
                                    else
                                    {
                                        hs.HS_HStatements.Clear();
                                    }

                                }

                                entity.Update_By = WorkContext.CurrentMembershipUser.Username;
                                entity.Update_Date = DateTime.Now;

                                uniOfWork.Commit();

                                model.NotificationType = "True";
                                PrepareAddHazardousSubstancesViewModel(model, hs);
                                return PartialView(model);
                            }
                            else
                            {
                                IList<MSDS_H_Statement> h_statements = new List<MSDS_H_Statement>();
                                if (model.HS_HStatement_Str != null)
                                {
                                    h_statements = _h_StatementService.GetListByNames(model.HS_HStatement_Str);
                                }
                                hs = new MSDS_HazardousSubstances
                                {
                                    HS_Id = Guid.NewGuid(),
                                    HS_Name = model.HS_Name?.Trim(),
                                    HS_MinPercent = model.HS_MinPercent,
                                    HS_MaxPercent = model.HS_MaxPercent,
                                    HS_CASCode = model.HS_CASCode?.Trim(),
                                    HS_HStatements = h_statements
                                };
                                entity.HazardousSubstance.Add(hs);
                                entity.Update_By = WorkContext.CurrentMembershipUser.Username;
                                entity.Update_Date = DateTime.Now;

                                uniOfWork.Commit();

                                model.NotificationType = "True";
                                PrepareAddHazardousSubstancesViewModel(model, null);

                                return PartialView(model);

                            }

                        }
                    }
                    else
                    {
                        model.Notification = "获取不到ID";
                        model.NotificationType = "False";
                        PrepareAddHazardousSubstancesViewModel(model, null);
                        return PartialView(model);
                    }

                }
                else
                {
                    model.Notification = "获取不到ID";
                    model.NotificationType = "False";
                    PrepareAddHazardousSubstancesViewModel(model, null);
                    return PartialView(model);
                }
            }
            else
            {
                model.Notification = "输入信息有误";
                model.NotificationType = "False";
                PrepareAddHazardousSubstancesViewModel(model, null);
                return PartialView(model);
            }

        }

        [HttpGet]
        public JsonResult GetCASCode()
        {
            try
            {
                IList<MSDS_Composition> compos = _compositionService.GetAll();
                List<object> list = new List<object>();
                foreach (var item in compos)
                {
                    list.Add(new {id=item.Composition_Name,Text=item.CASCode });
                }
                return new JsonResult { Data=list,JsonRequestBehavior= JsonRequestBehavior.AllowGet };
            }
            catch
            {
                return Json("");
            }
        }

        public ActionResult Import(Guid? id)
        {
            try
            {
                SpecificationImportViewModel viewModel = new SpecificationImportViewModel
                {
                    Id = id
                };
                return View(viewModel);
            }
            catch (Exception)
            {
                ErrorNotification(new Exception("上传失败，尝试上传无效的文件"));
                return View();
            }
        }
        [HttpPost]
        public ActionResult Import(SpecificationImportViewModel model)
        {
            try
            {
                MSDS_Specification entiy = new MSDS_Specification();
                var file = Request.Files["ImportExcelFile"];

                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType.Equals("application/pdf") || file.ContentType.Equals("text/xls"))
                    {
                        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                        {
                            if (model.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                            {
                                ErrorNotification(new Exception("请先保存危险品基本信息再行上传附件"));
                                return View(model);
                            }
                            else
                            {
                                Guid company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                                entiy = _specificationService.Single(model.Id.Value, company_Id);
                            }
                            char[] sp = new char[1] { '.' };
                            string[] strSuffix = file.FileName?.Split(sp);
                            if (strSuffix != null && strSuffix.Length == 2)
                            {

                                string fileName = entiy.Product_Name.Replace("%", " ").Replace(".", " ").Replace(@"/"," ") + "." + strSuffix[1];
                                //string fileName = DateTime.Now.Ticks.ToString() + "." + strSuffix[1];
                                string filePath = Server.MapPath("~/Content/Upload/" + fileName);

                                file.SaveAs(filePath);

                                entiy.AttachmentPath = "/Content/Upload/" + fileName;

                                unitOfWork.Commit();

                                SuccessNotification("上传成功！");

                                return View(model);
                            }
                            else
                            {
                                ErrorNotification(new Exception("上传失败，上传无效的文件"));
                                return View(model);
                            }
                        }
                    }
                    else
                    {
                        ErrorNotification(new Exception("上传失败，请上传PDF或EXCEL文件"));
                        return View(model);
                    }
                }
                else
                {
                    ErrorNotification(new Exception("上传失败，上传无效的文件"));
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return View(model);
            }
        }


        [HttpPost]
        public ActionResult DelHazardousSubstances(string sid, string hid)
        {
            try
            {
                Guid gSid = Guid.Parse(sid);
                Guid gHid = Guid.Parse(hid);

                using (var unitOfWordk = UnitOfWorkManager.NewUnitOfWork())
                {
                    Guid company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                    MSDS_Specification specification = _specificationService.Single(gSid, company_Id);
                    MSDS_HazardousSubstances hs = specification.HazardousSubstance.SingleOrDefault<MSDS_HazardousSubstances>(o => o.HS_Id == gHid);
                    specification.HazardousSubstance.Remove(hs);
                    _hazardousSubstancesService.Delete(hs);
                    unitOfWordk.Commit();
                }
                return Content("True");
            }
            catch (Exception)
            {
                return Content("False");
            }

        }

        public ActionResult DelSpecification(string sid)
        {
            try
            {
                Guid gSid = Guid.Parse(sid);
                using (var unitOfWordk = UnitOfWorkManager.NewUnitOfWork())
                {
                    Guid company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                    MSDS_Specification specification = _specificationService.Single(gSid, company_Id);
                    if (specification.WorkStations.Count > 1)
                    {
                        ErrorNotification(new Exception("有工位正在使用该化学品！请将化学品从该工位移除过后再行删除。"));
                    }
                    else
                    {
                        if (specification.HazardousSubstance.Count > 0)
                        {
                            _hazardousSubstancesService.DeleteList(specification.HazardousSubstance);
                        }
                        _specificationService.Delete(specification);
                        SuccessNotification("删除成功");
                        unitOfWordk.Commit();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult WaitForChecking()
        {
            try
            {
                MembershipUser user = WorkContext.CurrentMembershipUser;
                var model = new SpecificationManageViewModel();
                model.CheckStatus = 1;
                SearchOrders(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult WaitForChecking(SpecificationManageViewModel searchModel)
        {
            try
            {
                searchModel.CheckStatus = 1;
                switch (searchModel.ActionType)
                {
                    case "Download":
                        return Download(searchModel);
                    case "CheckValues":
                        string values = Request.Form["CheckValues"];
                        return CheckValues(values);
                    case "Search":
                    default:
                        SearchOrders(searchModel);
                        return View(searchModel);

                }
            }
            catch (Exception ex)
            {
                var model = new SpecificationManageViewModel();
                SearchOrders(model);
                ErrorNotification(ex);
                return View(model);
            }
        }

        public ActionResult ApplyCheck(Guid? id)
        {
            try
            {
                if (id != null)
                {
                    string productName = string.Empty;
                    using (var unitOfWordk = UnitOfWorkManager.NewUnitOfWork())
                    {
                        MembershipUser currentUser = WorkContext.CurrentMembershipUser;
                        Guid company_Id = currentUser.Company.Id;

                        var entity = _specificationCheckService.Single(id.Value);
                        if (entity != null)
                        {
                            entity.Status = 1;
                            entity.Update_By = currentUser.Username;
                            entity.Update_Date = DateTime.Now;
                        }
                        else
                        {
                            MSDS_Specification specification = _specificationService.Single(id.Value, company_Id);
                            productName = specification.CN_Name ?? specification.Product_Name;
                            MSDS_SpecificationCheck model = new MSDS_SpecificationCheck
                            {
                                Id = Guid.NewGuid(),
                                Specification = specification,
                                Status = 1,
                                Create_Date = DateTime.Now,
                                Create_By = currentUser.Username
                            };
                            _specificationCheckService.Add(model);
                        }

                        #region 发送请求审批邮件
                        Email email = PrepareEmail(id.Value, productName);
                        _emailService.Add(email);
                        #endregion

                        unitOfWordk.Commit();
                    }
                    
                    SuccessNotification("提交成功！");
                    return RedirectToAction("Index");
                }
                else
                {
                    ErrorNotification(new Exception("传入参数有误！"));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception("提交失败。" + ex.Message));
                return RedirectToAction("Index");
            }
        }

        public ActionResult DoCheck(Guid? id)
        {
            try
            {
                if (id != null)
                {
                    using (var unitOfWordk = UnitOfWorkManager.NewUnitOfWork())
                    {
                        MembershipUser currentUser = WorkContext.CurrentMembershipUser;
                        Guid company_Id = currentUser.Company.Id;
                        MSDS_Specification specification = _specificationService.Single(id.Value, company_Id);
                        if (specification.SpecificationCheck != null)
                        {
                            specification.SpecificationCheck.Status = 2;
                            specification.Update_By = currentUser.Username;
                            specification.Update_Date = DateTime.Now;
                        }
                        else
                        {
                            throw new Exception("未能找到该化学品的审批申请！");
                        }
                        unitOfWordk.Commit();
                        SuccessNotification("审批成功！");
                        return RedirectToAction("WaitForChecking");
                    }
                }
                else
                {
                    ErrorNotification(new Exception("请从正规入口进入该页面。"));
                    return RedirectToAction("WaitForChecking");
                }

            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception("审批失败。" + ex.Message));
                return RedirectToAction("WaitForChecking");
            }
        }

        public ActionResult RejectCheck(Guid? id)
        {
            try
            {
                if (id != null)
                {
                    SpecificationCheckViewModel model = new SpecificationCheckViewModel();
                    
                    using (var unitOfWordk = UnitOfWorkManager.NewUnitOfWork())
                    {
                        MembershipUser currentUser = WorkContext.CurrentMembershipUser;
                        var entity = _specificationCheckService.Single(id.Value);
                        if (entity != null)
                        {
                            model.Id = entity.Specification.Id;
                            model.RejectReason = entity.RejectReason;
                            model.Create_By = entity.Create_By;
                            model.Update_By = entity.Update_By;
                            model.Create_Date = entity.Create_Date;
                            model.Update_Date = entity.Update_Date;
                            model.Status = entity.Status;
                        }
                    }

                    return View(model);
                }
                else
                {
                    ErrorNotification(new Exception("请从正规入口进入该页面。"));
                    return RedirectToAction("WaitForChecking");
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception("网络错误。"+ex.Message));
                return RedirectToAction("WaitForChecking");
            }
        }

        [HttpPost]
        public ActionResult RejectCheck(SpecificationCheckViewModel model)
        {
            try
            {
                using (var unitOfWordk = UnitOfWorkManager.NewUnitOfWork())
                {
                    
                    MembershipUser currentUser = WorkContext.CurrentMembershipUser;
                    var entity = _specificationCheckService.Single(model.Id);
                    if (entity != null)
                    {
                        entity.Status = 3;
                        entity.RejectReason = model.RejectReason;
                        entity.Update_By = currentUser.Username;
                        entity.Update_Date = DateTime.Now;
                        unitOfWordk.Commit();
                    }
                    else
                    {
                        ErrorNotification(new Exception("驳回失败！未能找到对应的化学品。"));
                    }
                }
                SuccessNotification("驳回成功！");
                return RedirectToAction("WaitForChecking");
            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception("驳回失败。" + ex.Message));
                return View(model);
            }
        }

        public ActionResult CheckValues(string values)
        {
            try
            {
                
                if (!string.IsNullOrEmpty(values))
                {
                    char[] sp = new char[1] { ',' };
                    string[] arr = values.Split(sp,StringSplitOptions.RemoveEmptyEntries);

                    if (arr?.Length > 0)
                    {
                        int  errorCount = 0;
                        foreach (string item in arr)
                        {
                            #region do check
                            Guid id = Guid.Parse(item);
                            try
                            {
                                if (id != null)
                                {
                                    using (var unitOfWordk = UnitOfWorkManager.NewUnitOfWork())
                                    {
                                        MembershipUser currentUser = WorkContext.CurrentMembershipUser;
                                        Guid company_Id = currentUser.Company.Id;
                                        MSDS_Specification specification = _specificationService.Single(id, company_Id);
                                        if (specification.SpecificationCheck != null)
                                        {
                                            specification.SpecificationCheck.Status = 2;
                                            specification.Update_By = currentUser.Username;
                                            specification.Update_Date = DateTime.Now;
                                        }
                                        else
                                        {
                                            //throw new Exception("未能找到该化学品的审批申请！");
                                            errorCount++;
                                        }
                                        unitOfWordk.Commit();
                                    }
                                }
                                else
                                {
                                    errorCount++;
                                }
                            }
                            catch
                            {
                                errorCount++;
                            }
                            #endregion
                        }
                        if (errorCount==0)
                        {
                            SuccessNotification("全部审批成功！");
                        }
                        else
                        {
                            SuccessNotification("审批成功 " + (arr.Length - errorCount) + "个, 失败 " + errorCount + "个");
                        }
                        
                    }
                    else
                    {
                        ErrorNotification(new Exception("请选中至少一个化学品。"));
                    }
                    
                }
                else
                {
                    ErrorNotification(new Exception("请选中至少一个化学品。"));
                    
                }
                
            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception("批量审批失败。" + ex.Message));
            }
            return RedirectToAction("WaitForChecking");
        }

        public ActionResult AddComposition()
        {
            var model = new CompositionViewModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult AddComposition(CompositionViewModel model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    using (var unitOfWordk = UnitOfWorkManager.NewUnitOfWork())
                    {
                        var entity = new MSDS_Composition
                        {
                            CASCode = model.CASCode?.Trim(),
                            Composition_Name = model.Composition_Name?.Trim(),
                            Id = Guid.NewGuid()
                        };

                        _compositionService.Add(entity);
                        unitOfWordk.Commit();

                        SuccessNotification("保存成功");

                        return View(new CompositionViewModel());
                    }
                }
                else
                {
                    ErrorNotification(new Exception("保存失败"));
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return View(model);
            }
            
        }

        public ActionResult Detail(Guid? id= null,string cid = null)
        {
            if (id != null && cid != null)
            {

                Guid company_Id = Guid.Parse(cid);
                var entity = _specificationService.Single(id.Value, company_Id);
                if (entity != null)
                {
                    #region
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
                    PrepareSpecificationDetailViewModel(model, entity);
                    return View(model);
                    #endregion
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

        #endregion

        #region Utilities
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

            //List<string> trueOfFalse = new List<string>();
            //trueOfFalse.Add("是");
            //trueOfFalse.Add("否");

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

            #endregion


            if (entity != null)
            {
                model.Id = entity.Id;
                char[] sp = new char[1] { ',' };
                foreach (MSDS_H_Statement item in statments)
                {

                    #region 加载危险说明DropDownList
                    if (!string.IsNullOrEmpty(entity.GHS_HazardouDes_Values))
                    {
                        string[] tempArray = entity.GHS_HazardouDes_Values.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                        model.GHS_HazardouDes.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = ((tempArray != null) && tempArray.Any(o => o == item.Code)) });
                    }
                    else
                    {
                        model.GHS_HazardouDes.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = false });
                    }
                    #endregion
                }

                foreach (MSDS_P_Statement item in p_statments)
                {
                    #region 防范说明-预防DropDownList
                    string[] tempArray1 = entity.GHS_DefenceDes_Values?.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    model.GHS_DefenceDes.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = ((tempArray1 != null) && tempArray1.Any(o => o == item.Code)) });
                    #endregion

                    #region 防范说明-响应
                    string[] tempArray2 = entity.GHS_DealDES_Values?.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    model.GHS_DealDES.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = ((tempArray2 != null) && tempArray2.Any(o => o == item.Code)) });
                    #endregion

                    #region 防范说明-储存
                    string[] tempArray3 = entity.GHS_StoreDes_Values?.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    model.GHS_StoreDes.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = ((tempArray3 != null) && tempArray3.Any(o => o == item.Code)) });
                    #endregion
                }
            }
            else
            {
                foreach (MSDS_H_Statement item in statments)
                {
                    model.GHS_HazardouDes.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = false });

                }

                foreach (MSDS_P_Statement item in p_statments)
                {
                    model.GHS_DefenceDes.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = false });
                    model.GHS_DealDES.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = false });
                    model.GHS_StoreDes.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = false });
                }
            }

        }

        public void PrepareSpecificationDetailViewModel(SpecificationViewModel model, MSDS_Specification entity)
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

            //List<string> trueOfFalse = new List<string>();
            //trueOfFalse.Add("是");
            //trueOfFalse.Add("否");

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

            #endregion


            if (entity != null)
            {
                model.Id = entity.Id;
                char[] sp = new char[1] { ',' };
                foreach (MSDS_H_Statement item in statments)
                {

                    #region 加载危险说明DropDownList
                    string[] tempArray = entity.GHS_HazardouDes_Values?.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    if (tempArray != null && tempArray.Any(o => o == item.Code))
                    {
                        model.GHS_HazardouDes_String += item.Description + "\r\n";
                    }
                    #endregion
                }

                foreach (MSDS_P_Statement item in p_statments)
                {
                    #region 防范说明-预防DropDownList
                    string[] tempArray1 = entity.GHS_DefenceDes_Values?.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    if (tempArray1 != null && tempArray1.Any(o => o == item.Code))
                    {
                        model.GHS_DefenceDes_String += item.Description + "\r\n";
                    }
                    #endregion

                    #region 防范说明-响应
                    string[] tempArray2 = entity.GHS_DealDES_Values?.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    if (tempArray2 != null && tempArray2.Any(o => o == item.Code))
                    {
                        model.GHS_DealDES_String += item.Description + "\r\n";
                    }
                    #endregion

                    #region 防范说明-储存
                    string[] tempArray3 = entity.GHS_StoreDes_Values?.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    if (tempArray3 != null && tempArray3.Any(o => o == item.Code))
                    {
                        model.GHS_StoreDes_String += item.Description + "\r\n";
                    }
                    #endregion
                }
            }

        }

        private void PrepareAddHazardousSubstancesViewModel(HazardousSubstancesViewModel model, MSDS_HazardousSubstances entity)
        {
            IList<MSDS_H_Statement> statments = _h_StatementService.GetAll();
            IList<MSDS_Composition> compos = _compositionService.GetAll();

            List<SelectListItem> compos_sel = new List<SelectListItem>();
            if (entity != null)
            {
                foreach (MSDS_H_Statement item in statments)
                {
                    model.HS_HStatementSel.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = entity.HS_HStatements.Any(o => o.Code == item.Code) });
                }

                foreach (var item in compos)
                {
                    compos_sel.Add(new SelectListItem { Text = item.CASCode, Value= item.Composition_Name, Selected = item.CASCode == entity.HS_CASCode});
                }
            }
            else
            {
                foreach (MSDS_H_Statement item in statments)
                {
                    model.HS_HStatementSel.Add(new SelectListItem { Text = item.Code + ":" + item.Description, Value = item.Code, Selected = false });
                }

                foreach (var item in compos)
                {
                    compos_sel.Add(new SelectListItem { Text = item.CASCode, Value = item.Composition_Name});
                }
            }
            model.CASCodes = compos_sel;
        }

        private Email PrepareEmail(Guid sid,string product_Name)
        {
            IList<MembershipUser> users = MembershipService.GetAllUsersByComapany(WorkContext.CurrentMembershipUser.Company.Id);
            List<MembershipUser> checkers = users.Where(x => x.MembershipRoles.Any(o => o.MenuItems.Any(k => k.Id == 30))).ToList<MembershipUser>();

            int j = 1;
            string bcc = string.Empty;
            string toEmail = string.Empty;
            foreach (var item in checkers)
            {
                if (j == 1)
                {
                    toEmail = item.Email;
                }
                else
                {
                    bcc = item.Email + ";";
                }
                j++;
            }
            string emailBody = string.Empty;
            emailBody = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.IndexOf(Request.RawUrl)) + "/SpecificationCheck/WaitForChecking/" + sid.ToString();

            return new Email
            {
                Subject = "您有新的化学品需要审核",
                To = toEmail,
                ToName = WorkContext.CurrentMembershipUser.Username,
                Bcc = bcc,
                Body = string.Format(EmailBodyFormatter.EnterpriseBody, emailBody, product_Name),
                CreatedBy = WorkContext.CurrentMembershipUser.Company.CompanyName
            };


        }


        #endregion
    }
}