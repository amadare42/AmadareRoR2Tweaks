using BepInEx.Logging;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace AmadareTweaks.Logging
{
    public class ChatLogTarget : ILogTarget
    {
        public void Write(LogLevel level, object msg)
        {
            if (level == LogLevel.Debug || !Stage.instance) return;
            
            var color = level switch
            {
                LogLevel.Info => Color.blue,
                LogLevel.Warning => Color.yellow,
                LogLevel.Error => Color.red,
                LogLevel.Fatal => Color.red,
                _ => Color.blue
            };

            ChatMessage.SendColored(msg.ToString(), color);
        }
    }
}