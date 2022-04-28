﻿using BepInEx.Logging;

namespace AmadareTweaks.Logging
{
    public class ConsoleLoggerTarget : ILogTarget
    {
        private readonly ManualLogSource log;

        public ConsoleLoggerTarget(ManualLogSource log)
        {
            this.log = log;
        }

        public void Write(LogLevel level, object msg)
        {
            this.log.Log(level, msg);
        }
    }
}