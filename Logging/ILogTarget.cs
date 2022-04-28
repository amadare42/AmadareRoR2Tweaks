using BepInEx.Logging;

namespace AmadareTweaks.Logging
{
    public interface ILogTarget
    {
        void Write(LogLevel level, object msg);
    }
}