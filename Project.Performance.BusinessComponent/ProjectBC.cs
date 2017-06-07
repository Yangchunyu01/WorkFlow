using Project.Performance.Model;
using Project.Performance.Repository;

namespace Project.Performance.BusinessComponent
{
    public class ProjectBC : PerformanceBaseBC<ProjectModel>
    {
        public ProjectBC() : base(new ProjectCSOMClient())
        {
        }
    }
}
