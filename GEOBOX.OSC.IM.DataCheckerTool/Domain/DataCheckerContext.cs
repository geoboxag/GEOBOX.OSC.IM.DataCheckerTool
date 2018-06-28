using System;
using System.Diagnostics;

namespace GEOBOX.OSC.IM.DataCheckerTool.Domain
{
    public sealed class DataCheckerContext : IDataCheckerContext
    {
        private readonly Action<string> logMessageAction;

        public DataCheckerContext(Action<string> logMessageAction)
        {
            this.logMessageAction = logMessageAction ?? (_ => { });
        }

        public void LogMessage(string message, TraceLevel traceLevel)
        {
            switch (traceLevel)
            {
                case TraceLevel.Error:
                    {
                        logMessageAction(String.Format("[ERR]: {0}\r\n", message));
                        break;
                    }
                case TraceLevel.Info:
                    {
                        logMessageAction(String.Format("{0}\r\n", message));
                        break;
                    }
                case TraceLevel.Warning:
                    {
                        logMessageAction(String.Format("[WARN]: {0}\r\n", message));
                        break;
                    }
                default:
                        logMessageAction(String.Format("{0}\r\n", message));
                        break;
            }
        }
    }
}
