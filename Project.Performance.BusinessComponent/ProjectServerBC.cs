using Project.Performance.Model;
using Project.Performance.Repository;

namespace Project.Performance.BusinessComponent
{
    public class ProjectServerBC
    {
        private ProjectServerCSOMClient baseClient;

        public ProjectServerBC()
        {
            this.baseClient = new ProjectServerCSOMClient();
        }

        public bool Login(LoginInfo model)
        {
            return this.baseClient.Login(model);
        }

        public bool LoginNew(LoginInfo model)
        {
            return this.baseClient.LoginNew(model);
        }
    }
}
