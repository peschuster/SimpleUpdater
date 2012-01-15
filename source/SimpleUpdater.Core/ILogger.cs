using System;

namespace SimpleUpdater.Core
{
    /// <summary>
    /// Interface for logger.
    /// </summary>
    interface ILogger
    {
        void Error(string fromat, params object[] parameter);

        void Error(Exception exception);

        void Info(string format, params object[] parameter);
    }
}
