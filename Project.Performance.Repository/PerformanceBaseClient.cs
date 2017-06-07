using System;
using Microsoft.ProjectServer.Client;
using Project.Performance.Utility;
using System.Web;
using Project.Performance.Model;

namespace Project.Performance.Repository
{
    public abstract class PerformanceBaseClient
    {
        #region Properties
        public static ProjectContext BaseProjectContext
        {
            get; set;
            // get { return HttpContext.Current.Session[GlobeValue.ProjectContextSessionKey] as ProjectContext; }
            // set { HttpContext.Current.Session[GlobeValue.ProjectContextSessionKey] = value; }
        }

        // public TimeRecordInfo TimeRecord { get; set; }
        #endregion

        #region Constructors
        #endregion

        #region Methods
        protected bool ExcuteJobWithRetries(Action action, string actionStr)
        {
            bool result = true;
            DateTime startTime = DateTime.Now;
            while (true)
            {
                try
                {
                    action();
                    result = true;
                    break;
                }
                catch (Exception e)
                {
                    result = false;
                    ExceptionType exceptionType = ExceptionHelper.HandleExceptions(e.Message);
                    if (exceptionType == ExceptionType.SkipException)
                    {
                        break;
                    }
                    else if (exceptionType == ExceptionType.RetryException)
                    {
                        continue;
                    }
                    else if (exceptionType == ExceptionType.SkipOrRetryException)
                    {
                        if (DateTime.Now - startTime < new TimeSpan(0, 3, 0))
                        {
                            LoggerHelper.Instance.WriteLine(string.Format("Warning {0}.\nMessage: {1}. \nMore:{2} \nRetrying...", actionStr, e.Message, e.StackTrace), LogType.Warning);
                            continue;
                        }
                        else
                        {
                            LoggerHelper.Instance.WriteLine("Abort this task's creation, go for next one", LogType.Comment);
                            break;
                        }
                    }
                    else
                    {
                        LoggerHelper.Instance.WriteLine(string.Format("A new unexpected exception that we didn't meet before related to {0}. \nMessage:{1}. \nMore:{2}", actionStr, e.Message, e.StackTrace));
                        break;
                    }
                }
            }
            return result;
        }

        protected void WaitForJobComplete(QueueJob job, int? seconds = null)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                int waitTime = seconds ?? 300;
                JobState jobState = JobState.Unknown;
                do
                {
                    jobState = BaseProjectContext.WaitForQueue(job, 3);
                }
                while (jobState != JobState.Success && startTime.AddSeconds(waitTime) > DateTime.Now);

                if (jobState == JobState.Success)
                {
                    LoggerHelper.Instance.Pass(job.Id.ToString());
                }
                else
                {
                    LoggerHelper.Instance.Fail(string.Format("The job status is {0}.", jobState));
                }
            }
            catch (Exception e)
            {
                job.Cancel();
                LoggerHelper.Instance.Fail(string.Format("Warning: The job is failed.\nMessage: {0}. \nMore:{1} \nRetrying...", e.Message, e.StackTrace));
            }
        }
        #endregion
    }
}
