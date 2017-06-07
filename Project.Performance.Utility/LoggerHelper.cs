using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Project.Performance.Model;

namespace Project.Performance.Utility
{
    public class LoggerHelper
    {
        #region Variables
        private static LoggerHelper logger;
        private static string strLock = string.Empty;

        private static string fileFolder;
        private static string fileName;
        private static StreamWriter writer;
        #endregion

        #region Properties
        public static LoggerHelper Instance
        {
            get
            {
                if (LoggerHelper.logger == null)
                {
                    lock (LoggerHelper.strLock)
                    {
                        if (LoggerHelper.logger == null)
                        {
                            LoggerHelper.logger = new LoggerHelper();
                        }
                    }
                }
                return LoggerHelper.logger;
            }
        }

        public static string FileName { get { return LoggerHelper.fileName; } }

        public string Text { get; private set; }
        #endregion

        #region Constructors
        private LoggerHelper()
        {
            // Create log file
            LoggerHelper.fileFolder = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), BaseConfig.LogFolderPath);
            LoggerHelper.fileName = DateTime.Now.ToString(GlobeValue.LogFileNameFormat) + ".txt";
            DirectoryInfo di = new DirectoryInfo(LoggerHelper.fileFolder);
            if (!di.Exists)
            {
                di.Create();
            }

            // Create stream writer
            LoggerHelper.writer = new StreamWriter(Path.Combine(LoggerHelper.fileFolder, LoggerHelper.fileName), true);
        }
        #endregion

        #region Methods

        public void WriteLine(string message, LogType logType = LogType.None)
        {
            string line = string.Format("{0}\t{1}\t{2}", DateTime.Now.ToString(GlobeValue.LogTextFormat, CultureInfo.InvariantCulture), logType, message);
            Text += line + "\r\n";
            LoggerHelper.writer.WriteLine(line);
            LoggerHelper.writer.Flush();
        }

        public void EnterMethod(MethodInfo method)
        {
            WriteLine("Entering method: " + method.Name + " (" + string.Join(", ", method.GetParameters().Select(x => x.ToString())) + ")"
                + " called by type: " + method.DeclaringType, LogType.Comment);
        }

        public void LeaveMethod(MethodInfo method)
        {
            WriteLine("Leaving method: " + method.Name + "(" + string.Join(", ", method.GetParameters().Select(x => x.ToString())) + ")"
                + " called by type: " + method.DeclaringType, LogType.Comment);
        }

        public void Comment(string message)
        {
            WriteLine(message, LogType.Comment);
        }

        public void Pass(string message)
        {
            WriteLine(message, LogType.Pass);
        }

        public void Fail(string message)
        {
            WriteLine(message, LogType.Fail);
        }

        public void Error(Exception exception)
        {
            WriteLine(exception.ToString(), LogType.Exception);
            WriteLine(exception.StackTrace, LogType.None);
        }

        public void Verify(int actualValue, int expectedValue)
        {
            if (actualValue == expectedValue)
            {
                Pass(String.Format("This verification passes as expected, actualValue{0}, expectedValue{1}", actualValue, expectedValue));
            }
            else
            {
                Fail(String.Format("This verification failed, actualValue{0}, expectedValue{1}", actualValue, expectedValue));
            }
        }
        #endregion
    }

    public enum LogType
    {
        None,
        Comment,
        Pass,
        Fail,
        Warning,
        Exception
    }
}
