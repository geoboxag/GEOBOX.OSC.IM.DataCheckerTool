using System.Diagnostics;

namespace GEOBOX.OSC.IM.DataCheckerTool.Domain
{
    public interface IDataCheckerContext
    {
        void LogMessage(string message, TraceLevel traceLevel);
    }
}
