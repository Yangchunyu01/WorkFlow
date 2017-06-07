using Project.Performance.BusinessComponent;
using Project.Performance.Model;

namespace Project.Performance.Service
{
    public class ProjectServerService
    {
        private ProjectServerBC bc;
        private static ProjectServerService instance;


        private ProjectServerService()
        {
            this.bc = new ProjectServerBC();
        }

        public static ProjectServerService Instance
        {
            get
            {
                if (ProjectServerService.instance == null)
                {
                    ProjectServerService.instance = new ProjectServerService();
                }
                return ProjectServerService.instance;
            }
        }

        public bool Login(LoginInfo model)
        {
            return this.bc.Login(model);
        }

        public bool LoginNew(LoginInfo model)
        {
            return this.bc.LoginNew(model);
        }
    }
}
