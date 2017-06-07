using Project.Performance.BusinessComponent;
using Project.Performance.Model;

namespace Project.Performance.Service
{
    [ServiceAPI(typeof(ProjectModel))]
    public class ProjectService : IPerformanceBaseService<ProjectModel>
    {
        private ProjectBC bc = new ProjectBC();
        public bool Create(ProjectModel model)
        {
            return this.bc.Create(model);
        }

        public bool Delete(ProjectModel model)
        {
            return this.bc.Delete(model);
        }

        public int GetCount(XMLModel model)
        {
            return this.bc.GetCount(model);
        }
    }
}
