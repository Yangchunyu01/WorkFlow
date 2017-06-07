using Project.Performance.Model;
using Project.Performance.Service;
using System.Web.Mvc;

namespace Project.Performance.WebApp.Controllers
{
    public class PerformanceBaseController : Controller
    {
        protected static XMLModel xmlModel;
        protected static ConfigService configService;
        protected static ProjectServerService serverService;

        public PerformanceBaseController()
        {
            PerformanceBaseController.serverService = ProjectServerService.Instance;
            PerformanceBaseController.configService = ConfigService.Instance;
        }

        protected internal new JsonResult Json(object data, JsonRequestBehavior behavior)
        {
            JsonResult result = base.Json(data, behavior);
            return result;
        }
        protected string GlobalUserName
        {
            get
            {
                return User.Identity.Name.IndexOf('\\') != -1 ? User.Identity.Name.Split('\\')[1] : User.Identity.Name;
            }
            private set
            {
            }
        }
    }
}