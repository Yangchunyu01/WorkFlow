using System.Web.Mvc;

namespace Project.Performance.WebApp.Controllers
{
    public class ViewController : PerformanceBaseController
    {
        public ActionResult Index()
        {
            this.ViewBag.Data = GlobalUserName;
            return View("Index");
        }

        public ActionResult Login()
        {
            return PartialView("Login");
        }

        public ActionResult NewProjectAndTasks()
        {
            return PartialView("_NewProjectAndTasks");
        }
        public ActionResult NewResources()
        {
            return PartialView("_NewResources");
        }

        public ActionResult NewCustomFields()
        {
            return PartialView("_NewCustomFields");
        }
        public ActionResult NewLookupTables()
        {
            return PartialView("_NewLookupTables");
        }
        public ActionResult LoadUserInfo()
        {
            return View("_UserInfo");
        }
        public ActionResult ShowSaveFilePartial()
        {
            return PartialView("_SaveFileDialog");
        }

        public ActionResult ShowOpenFilePartial()
        {
            return PartialView("_OpenFileDialog");
        }
    }
}