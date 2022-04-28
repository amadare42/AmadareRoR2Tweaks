using System;
using System.Collections.Generic;
using AmadareTweaks.Configuration;
using AmadareTweaks.Logging;
using AmadareTweaks.Tweaks;
using BepInEx;
using BepInEx.Configuration;
using MonoMod.RuntimeDetour;
using RoR2;
using UnityEngine.SceneManagement;

namespace AmadareTweaks
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.rune580.riskofoptions")]
    public class AmadareTweaksPlugin : BaseUnityPlugin
    {
        private DetourModManager detourModManager;
        private PluginConfig pluginConfig;

        public List<Tweak> Tweaks { get; set; } = new List<Tweak>();

        private void Awake()
        {
            detourModManager = new DetourModManager();
            
            Log.Init(Logger);
            this.pluginConfig = PluginConfig.Load(this.Config);
            Log.ApplyConfig(this.pluginConfig);
            
            this.Tweaks = CreateTweaks();
            ApplyTweaksConfig();
            RegisterTweaks();
            SceneManager.sceneLoaded += OnSceneManagerOnsceneLoaded;
            this.Config.SettingChanged += OnSettingChanged;
            
            // initialize everything if awoken in the middle of stage - i.e. when reloading when game is running
            if (Stage.instance)
            {
                Logger.LogInfo("Stage is running - applying tweaks");
                RegisterTweaks();
            }

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void OnSettingChanged(object sender, SettingChangedEventArgs e)
        {
            Logger.LogInfo("Config changed - updating tweaks");
            ApplyTweaksConfig();
        }

        private void OnSceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SceneChanged();
        }

        public List<Tweak> CreateTweaks()
        {
            return new List<Tweak>
            {
                new OtherPlayerItemsTweak(),
                // new MadDirectorTweak(),
                new PriorityBeadsTweak(),
                new CommandKeyAlwaysTweak()
            };
        }

        public void ApplyTweaksConfig()
        {
            foreach (var tweak in this.Tweaks)
            {
                try
                {
                    tweak.ConfigChanged(this.pluginConfig);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error during setting config for '{tweak.GetType().Name}' tweak: {ex}");
                }
            }
        }
        
        public void RegisterTweaks()
        {
            foreach (var tweak in this.Tweaks)
            {
                try
                {
                    tweak.Register();
                }
                catch (Exception ex)
                {
                    Log.Error($"Error during registration of '{tweak.GetType().Name}' tweak: {ex}");
                }
            }
        }

        public void SceneChanged()
        {
            foreach (var tweak in this.Tweaks)
            {
                try
                {
                    tweak.OnSceneChange();
                }
                catch (Exception ex)
                {
                    Log.Error($"Error on scene change handling for '{tweak.GetType().Name}' tweak: {ex}");
                }
            }
        }

        public void DisposeTweaks()
        {
            foreach (var tweak in this.Tweaks)
            {
                try
                {
                    tweak.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Error($"Error during disposing of '{tweak.GetType().Name}' tweak: {ex}");
                }
            }
        }

        private void OnDestroy()
        {
            this.detourModManager.Unload(GetType().Assembly);
            SceneManager.sceneLoaded -= OnSceneManagerOnsceneLoaded;
            DisposeTweaks();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} was unloaded!");
        }
    }
}
