using System;
using Project.Performance.Model;
using Project.Performance.Repository;
using Project.Performance.Utility;

namespace Project.Performance.BusinessComponent
{
    public abstract class PerformanceBaseBC<T> where T : PerformanceBaseModel
    {
        public ICSOMClient<T> baseClient { get; set; }

        public PerformanceBaseBC(ICSOMClient<T> client)
        {
            baseClient = client;
        }

        // Create
        private K ExecuteAPITask<K>(Func<T, K> func, T para)
        {
            try
            {
                LoggerHelper.Instance.EnterMethod(func.Method);
                return func(para);
            }
            catch (Exception e)
            {
                LoggerHelper.Instance.Error(e);
            }
            finally
            {
                LoggerHelper.Instance.LeaveMethod(func.Method);
            }

            return default(K);
        }

        // Get Count
        private P ExecuteAPITask<K, P>(Func<K, P> func, K para)
        {
            try
            {
                LoggerHelper.Instance.EnterMethod(func.Method);
                return func(para);
            }
            catch (Exception e)
            {
                LoggerHelper.Instance.Error(e);
            }
            finally
            {
                LoggerHelper.Instance.LeaveMethod(func.Method);
            }
            return default(P);
        }

        public bool Create(T model)
        {
            return ExecuteAPITask<bool>(this.baseClient.Create, model);
        }

        public bool Delete(T model)
        {
            return ExecuteAPITask<bool>(this.baseClient.Delete, model);
        }

        public int GetCount(XMLModel model)
        {
            return ExecuteAPITask<XMLModel, int>(this.baseClient.GetCount, model);
        }
    }
}
