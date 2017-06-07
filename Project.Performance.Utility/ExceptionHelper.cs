using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Performance.Utility
{
    public enum ExceptionType
    {
        UnHandleException,
        SkipException, // Must skip
        RetryException, // Must retry
        SkipOrRetryException // Skip after some retry
    }

    public class ExceptionHelper
    {
        private const string PROJ_NAME_EXIST_EXP = "ProjectNameAlreadyExists";
        private const string PROJ_UPDATE_DB_EXP = "ProjectUpdateDatabaseException";
        private const string DB_READONLY_EXP = "GeneralDalDatabaseIsReadOnly";
        private const string DB_UNDEFERROR_EXP = "DatabaseUndefinedError";
        private const string QUEUE_BLOCKED_EXP = "GeneralQueueCorrelationBlocked";
        private const string SEQUENCE_NO_ELEMENT_EXP = "Sequence contains no elements";
        private const string UNAUTHORIZED_401_EXP = "(401) Unauthorized";
        private const string TIMEOUT_EXP = "timed out";

        public static ExceptionType HandleExceptions(string exceptionMessage)
        {
            ExceptionType handleException = ExceptionType.UnHandleException;
            FieldInfo fi = typeof(ExceptionHelper).GetRuntimeFields().FirstOrDefault(x => exceptionMessage.Contains(x.GetValue(null).ToString()));
            if (fi == null)
            {
                return handleException;
            }
            string exceptionName = fi.Name;

            if (exceptionName != null)
            {
                switch (exceptionName)
                {
                    case "PROJ_NAME_EXIST_EXP":
                        LoggerHelper.Instance.WriteLine("ProjectNameAlreadyExists issue, skip to create a new project", LogType.Warning);
                        handleException = ExceptionType.SkipException;
                        break;
                    case "PROJ_UPDATE_DB_EXP":
                        LoggerHelper.Instance.WriteLine("ProjectUpdateDatabaseException issue, retry actions...", LogType.Warning);
                        handleException = ExceptionType.SkipOrRetryException;
                        break;

                    case "DB_READONLY_EXP":
                        LoggerHelper.Instance.WriteLine("GeneralDalDatabaseIsReadOnly issue: Database is readonly, abort execution", LogType.Warning);
                        handleException = ExceptionType.SkipException;
                        break;
                    case "DB_UNDEFERROR_EXP":
                        LoggerHelper.Instance.WriteLine("DatabaseUndefinedError issue, retry actions...", LogType.Warning);
                        handleException = ExceptionType.SkipOrRetryException;
                        break;

                    case "UNAUTHORIZED_401_EXP":
                        LoggerHelper.Instance.WriteLine("(401) Unauthorized issue, retry to actions...", LogType.Warning);
                        handleException = ExceptionType.RetryException;
                        break;
                    case "SEQUENCE_NO_ELEMENT_EXP":
                        LoggerHelper.Instance.WriteLine("Sequence contains no elements issue, retry actions...", LogType.Warning);
                        handleException = ExceptionType.SkipOrRetryException;
                        break;
                    case "TIMEOUT_EXP":
                        LoggerHelper.Instance.WriteLine("timed out issue from server, abort actions...", LogType.Warning);
                        handleException = ExceptionType.SkipException;
                        break;
                    case "QUEUE_BLOCKED_EXP":
                        LoggerHelper.Instance.WriteLine("GeneralQueueCorrelationBlocked issue, retry actions...", LogType.Warning);
                        handleException = ExceptionType.SkipOrRetryException;
                        break;
                    default:
                        handleException = ExceptionType.UnHandleException;
                        break;
                }
            }
            return handleException;
        }
    }
}
