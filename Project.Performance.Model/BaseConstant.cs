using System.Configuration;

namespace Project.Performance.Model
{
    public class BaseConstant
    {
    }

    public class BaseConfig
    {
        public static string SaveFilePath = ConfigurationManager.AppSettings["SaveFilePath"];
        public static string LogFolderPath = ConfigurationManager.AppSettings["LogFolder"];
    }

    public class GlobeValue
    {
        public const string ProjectModelStartNode = "<ProjectTemplate>";
        public const string ProjectModelEndNode = "</ProjectTemplate>";

        public const string LogFileNameFormat = "yyyy-MM-dd-HH-mm-ss";
        public const string LogTextFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public const string ProjectContextSessionKey = "ProjectContext";
    }
}
