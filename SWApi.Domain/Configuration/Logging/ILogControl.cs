using NLog;

namespace SWApi.Domain.Configuration.Logging;

public interface ILogControl
{
    Logger GetLogger(string name);
}