using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System.Reflection;
using System.Text;

namespace SWApi.Domain.Configuration.Logging
{
    public class LogControl : ILogControl
    {
        private const string _defaultLogLevel = "Info";
        public string UtcOffset { get; set; } = "-03:00";
        public string LogLevel { get; set; }

        public LogControl(string logLevel)
        {
             ConfigureLog(logLevel);
        }

        private static void ConfigureLog(string logLevel)
        {
            if (string.IsNullOrWhiteSpace(logLevel))
                logLevel = _defaultLogLevel;

            var jsonLayout = new JsonLayout();
            jsonLayout.Attributes.Add(new JsonAttribute("date", Layout.FromString("${longdate}")));
            jsonLayout.Attributes.Add(new JsonAttribute("level", Layout.FromString("${level:upperCase=true}")));
            jsonLayout.Attributes.Add(new JsonAttribute("message", Layout.FromString("${message}")));
            jsonLayout.Attributes.Add(new JsonAttribute("file", Layout.FromString("${callsite}:${callsite-linenumber}")));
            jsonLayout.Attributes.Add(new JsonAttribute("exception", Layout.FromString("${exception:format=ToString}")));

            var solutionRootPath = Path.GetFullPath("..\\logs");
            var logFullPath = solutionRootPath + $"\\logs_{DateTime.Today:yyyyMMdd}.log";

            var logfile = new FileTarget("log")
            {
                FileName = logFullPath,
                CreateDirs = true,
                Layout = jsonLayout,
                Encoding = Encoding.UTF8
            };

            LoggingConfiguration loggingConfiguration = new();

            loggingConfiguration.AddTarget(logfile);
            loggingConfiguration.AddRule(NLog.LogLevel.FromString(logLevel), NLog.LogLevel.Fatal, logfile);

            LogManager.Configuration = loggingConfiguration;
        }

        public ILogger GetLogger(string name)
        {
            return LogManager.GetLogger(name);
        }
    }
}
