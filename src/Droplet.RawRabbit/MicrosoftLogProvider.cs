using Microsoft.Extensions.Logging;
using RawRabbit.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using LogLevel = RawRabbit.Logging.LogLevel;

namespace Droplet.RawRabbit
{
    internal class MicrosoftLogProvider : ILogProvider
    {
        private readonly Func<string, ILogger> _createLoggerFunc;
        public MicrosoftLogProvider(Func<string, ILogger> createLoggerFunc)
        {
            _createLoggerFunc = createLoggerFunc;
        }

        public Logger GetLogger(string name)
        {
            var logger = _createLoggerFunc(name);

            Logger delegateLogger = (level, messageFunc, exception, formatParameters) => {
                var mslevel = TranslateLevel(level);

                if (messageFunc == null)
                {
                    return logger.IsEnabled(mslevel);
                }
                logger.Log(logLevel: mslevel, exception:exception, message : messageFunc.Invoke(), args: formatParameters);
                
                return true;
            };
            return delegateLogger;
        }

        private Microsoft.Extensions.Logging.LogLevel TranslateLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return Microsoft.Extensions.Logging.LogLevel.Trace;
                    break;
                case LogLevel.Debug:
                    return Microsoft.Extensions.Logging.LogLevel.Debug;
                    break;
                case LogLevel.Info:
                    return Microsoft.Extensions.Logging.LogLevel.Information;
                    break;
                case LogLevel.Warn:
                    return Microsoft.Extensions.Logging.LogLevel.Warning;
                    break;
                case LogLevel.Error:
                    return Microsoft.Extensions.Logging.LogLevel.Error;
                    break;
                case LogLevel.Fatal:
                    return Microsoft.Extensions.Logging.LogLevel.Critical;
                    break;
                default:
                    return Microsoft.Extensions.Logging.LogLevel.None;
                    break;
            }
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }
    }
    
}
