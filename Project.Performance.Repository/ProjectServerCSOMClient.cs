using System.Linq;
using System.Net;
using System.Security;
using Microsoft.ProjectServer.Client;
using Microsoft.SharePoint.Client;
using Project.Performance.Model;
using Project.Performance.Utility;

namespace Project.Performance.Repository
{
    public class ProjectServerCSOMClient : PerformanceBaseClient
    {
        public bool Login(LoginInfo model)
        {
            BaseProjectContext = new ProjectContext(model.Url);
            NetworkCredential netcred = new NetworkCredential(model.UserName, model.Password);
            SharePointOnlineCredentials orgIDCredential = new SharePointOnlineCredentials(netcred.UserName, netcred.SecurePassword);
            BaseProjectContext.Credentials = orgIDCredential;
            try
            {
                BaseProjectContext.ExecuteQuery();
                return true;
            }
            catch (IdcrlException e)
            {
                LoggerHelper.Instance.Error(e);
                return false;
            }
        }

        public bool LoginNew(LoginInfo model)
        {
            BaseProjectContext = new ProjectContext(model.Url);
            SecureString psw = new SecureString();
            model.Password.ToList().ForEach(item => psw.AppendChar(item));
            BaseProjectContext.Credentials = new SharePointOnlineCredentials(model.UserName, psw);
            User user = BaseProjectContext.Web.CurrentUser;
            try
            {
                BaseProjectContext.Load(user);
                BaseProjectContext.ExecuteQuery();
                return true;
            }
            catch (IdcrlException e)
            {
                LoggerHelper.Instance.Error(e);
                return false;
            }
        }
    }
}
