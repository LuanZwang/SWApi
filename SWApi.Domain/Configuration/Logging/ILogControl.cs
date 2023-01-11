using NLog;

namespace SWApi.Domain.Configuration.Logging
{
    public interface ILogControl
    {
        ILogger GetLogger(string name);
    }
}