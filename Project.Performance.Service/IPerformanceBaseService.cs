using Project.Performance.Model;

namespace Project.Performance.Service
{
    public interface IPerformanceBaseService<T> where T : PerformanceBaseModel
    {
        bool Create(T model);

        bool Delete(T model);

        int GetCount(XMLModel model);
    }
}
