using System;
using AmadareTweaks.Configuration;

namespace AmadareTweaks.Tweaks;

public class Tweak : IDisposable
{
    protected PluginConfig Config;

    public virtual void ConfigChanged(PluginConfig config)
    {
        this.Config = config;
    }
    
    public virtual void Register()
    {
    }

    public virtual void OnSceneChange()
    {
        
    }

    public virtual void Dispose()
    {
    }
}