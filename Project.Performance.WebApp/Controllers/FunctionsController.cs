using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Performance.Model;
using Project.Performance.Utility;
using Project.Performance.WebApp.ViewModels;

namespace Project.Performance.WebApp.Controllers
{
    public class FunctionsController : PerformanceBaseController
    {
        public ActionResult LoginServer(string serverUrl, string userName, string passWord)
        {
            try
            {
                LoginInfo loginModel = new LoginInfo(serverUrl, userName, passWord);
                bool isLogin = serverService.Login(loginModel);
                if (isLogin)
                {
                    return Json(new JsonMessage(true, "Log in successful!"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new JsonMessage(false, "The sign-in name or password does not match one in the Microsoft account system."), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonMessage(false, "FAILED:" + ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateInput(false)]
        public ActionResult ExecuteConfig(string models)
        {
            try
            {
                xmlModel = XMLHelper.GetModelFromXML(models);
                configService.ExecuteConfigTasks(xmlModel);
                return Json(new JsonMessage(true, "Execute action is triggered."), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonMessage(false, "FAILED:" + ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        #region Refresh Functions
        public ActionResult GetResourceCreationStatus()
        {
            try
            {
                if (xmlModel != null && xmlModel.ResourcePoolModelList != null)
                {
                    List<ResourcePoolModel> models = xmlModel.ResourcePoolModelList;
                    if (models.Count > 0)
                    {
                        int all = models.Sum(item => item.Count);
                        int finish = configService.ResourceService.GetCount(xmlModel);
                        if (finish > all) { finish = all; }
                        double resourceCreationTime = 0; // configService.ResourceService.TimeRecord
                        return Json(
                            new
                            {
                                All = all,
                                Finished = finish,
                                CreationTime = resourceCreationTime,
                                Prefix = string.Join(",", models.Select(item => item.NamePrefix)),
                                PoolCount = finish,
                                log = LoggerHelper.Instance.Text
                            }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(
                    new
                    {
                        All = 0,
                        Finished = 0,
                        CreationTime = 0,
                        Prefix = string.Empty
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { data = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetProjectCreationStatus()
        {
            try
            {
                if (xmlModel != null && xmlModel.ProjectModelList != null)
                {
                    List<ProjectModel> models = xmlModel.ProjectModelList;
                    if (models.Count > 0)
                    {
                        List<ProjectCreationStatusViewModel> projStatus = new List<ProjectCreationStatusViewModel>();
                        int all = models.Sum(item => item.Count);
                        int finish = configService.ProjectService.GetCount(xmlModel);
                        projStatus.Add(new ProjectCreationStatusViewModel()
                        {
                            All = all,
                            NamePrefix = string.Join(",", models.Select(item => item.NamePrefix)),
                            Finished = finish
                        });

                        return Json(
                            new
                            {
                                ProjCreationInfo = projStatus,
                                Total = finish,
                                log = LoggerHelper.Instance.Text
                            }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(
                    new
                    {
                        Total = 0,
                        ProjCreationInfo = new ProjectCreationStatusViewModel[0],
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { data = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetLookupTableCreationStatus()
        {
            try
            {
                if (xmlModel != null && xmlModel.LookupTableModelList != null)
                {
                    List<LookupTableModel> models = xmlModel.LookupTableModelList;
                    if (models.Count > 0)
                    {
                        int all = models.Sum(item => item.Count);
                        int finish = configService.LookupTableService.GetCount(xmlModel);
                        double LookupTableCreationTime = 0;

                        return Json(
                            new
                            {
                                All = all,
                                Finished = finish,
                                CreationTime = LookupTableCreationTime,
                                Prefix = string.Join(",", models.Select(item => item.Name)),
                                log = LoggerHelper.Instance.Text
                            }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(
                    new
                    {
                        All = 0,
                        Finished = 0,
                        CreationTime = 0,
                        Prefix = string.Empty
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { data = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCustomFieldStatus()
        {
            try
            {
                if (xmlModel != null && xmlModel.CustomFieldModelList != null)
                {
                    List<CustomFieldModel> models = xmlModel.CustomFieldModelList;
                    if (models.Count > 0)
                    {
                        int all = models.Sum(item => item.Count);
                        int finish = configService.CustomFieldService.GetCount(xmlModel);
                        double customFieldsCreationTime = 0;

                        return Json(
                        new
                        {
                            All = all,
                            Finished = finish,
                            Prefix = string.Join(",", models.Select(item => item.Name)),
                            CreationTime = customFieldsCreationTime,
                            log = LoggerHelper.Instance.Text
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(
                    new
                    {
                        All = 0,
                        Finished = 0,
                        CreationTime = 0,
                        Prefix = string.Empty
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { data = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion Refresh Functions

        #region Icon buttons functions
        [ValidateInput(false)]
        public ActionResult SaveFile(string model, string filename)
        {
            try
            {
                // Judge whether the format of xml is legal before saving 
                bool isValidated = XMLHelper.ValidateXMLFormat(model);
                if (!isValidated)
                {
                    return Json(new JsonMessage(false, "FAILED: The format of xml is incorrect!"), JsonRequestBehavior.AllowGet);
                }
                XMLHelper.SaveStringToFile(model, filename, GlobalUserName);
                return Json(new JsonMessage(true, "Save File Successfully"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonMessage(false, "FAILED:" + ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Import()
        {
            if (Request.Files != null && Request.Files.Count > 0)
            {
                try
                {
                    HttpPostedFileBase file = Request.Files[0];
                    return Json(new { data = XMLHelper.FormatXML(file.InputStream) }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new JsonMessage(false, "FAILED:" + ex.Message), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new JsonMessage(false, "Import file is empty!"));
            }
        }

        public ActionResult OpenRemoteSampleFile()
        {
            string path = @"\\Jin-Pt001\config";
            string dosline = @"explorer.exe " + path;
            try
            {
                using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
                {
                    proc.StartInfo.FileName = "cmd.exe";

                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.CreateNoWindow = true;

                    proc.Start();
                    proc.StandardInput.WriteLine(dosline);
                    proc.StandardInput.WriteLine("exit");
                    while (!proc.HasExited)
                    {
                        proc.WaitForExit(100);
                    }
                }

                return Json(
                    new { }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { data = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion Icon buttons functions
    }
}