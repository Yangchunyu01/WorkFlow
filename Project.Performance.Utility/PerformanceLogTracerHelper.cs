using System;

namespace Project.Performance.Utility
{
    public class PerformanceLogTracerHelper
    {
        #region Variables
        private static PerformanceLogTracerHelper tracer;
        private static string strLock = string.Empty;
        #endregion

        private PerformanceLogTracerHelper()
        { }

        public static PerformanceLogTracerHelper Instance
        {
            get
            {
                if (PerformanceLogTracerHelper.tracer == null)
                {
                    lock (PerformanceLogTracerHelper.strLock)
                    {
                        if (PerformanceLogTracerHelper.tracer == null)
                        {
                            PerformanceLogTracerHelper.tracer = new PerformanceLogTracerHelper();
                        }
                    }
                }
                return PerformanceLogTracerHelper.tracer;
            }
        }
    }
}
