using System;
using System.Diagnostics;

namespace SimpleUpdater.Core
{
    internal class TraceLogger : ILogger
    {
        static TraceLogger()
        {
            string fileName = "UpdateLog_" + DateTime.Now.ToString() + ".log";
            fileName = fileName.Replace(":", "-");
            fileName = fileName.Replace(".", "-");
            fileName = fileName.Replace(",", "-");
            fileName = fileName.Replace("/", "-");
            fileName = fileName.Replace(@"\", "-");
            fileName = fileName.Replace(" ", "_");

            Trace.Listeners.Add(new TextWriterTraceListener(fileName));
            Trace.AutoFlush = true;
        }

        public void Error(string fromat, params object[] parameter)
        {
            try
            {
                Trace.TraceError(fromat, parameter);
            }
            catch (Exception)
            {
            }
        }

        public void Error(Exception exception)
        {
            try 
            {
                Trace.TraceError(exception.ToString());
            }
            catch (Exception)
            {
            }
        }

        public void Info(string format, params object[] parameter)
        {
            try
            {
                if (parameter == null)
                {
                    Trace.TraceInformation(format);
                }
                else
                {
                    Trace.TraceInformation(String.Format(format, parameter));
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
