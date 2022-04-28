using System;
using System.Collections.Generic;
using System.Reflection;
using AmadareTweaks.Logging;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;

namespace AmadareTweaks.Configuration
{
    public static class ConfigFileExtensions
    {
        public static BindCollection BindCollection(this ConfigFile config, string section = null)
        {
            return new BindCollection(config, section);
        }
    }

    public class BindCollection
    {
        private readonly ConfigFile config;
        public string Section { get; private set; }

        public List<ConfigEntryBase> Bindings { get; } = new();
        private Dictionary<ConfigEntryBase, Action> UpdateCallbacks { get; } = new();

        public event Action OnChanged;

        public BindCollection(ConfigFile config, string section)
        {
            this.config = config;
            this.Section = section;
            this.config.SettingChanged += OnSettingChanged;
        }

        private void OnSettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (this.UpdateCallbacks.TryGetValue(e.ChangedSetting, out var cb))
            {
                cb();
                this.OnChanged?.Invoke();
            }
        }

        public BindCollection Bind(Action<BindCollection> registrar)
        {
            registrar(this);
            return this;
        }

        public BindCollection Bind<TValue>(string key,
            string description,
            Action<TValue> set,
            TValue defaultValue = default,
            Func<ConfigEntry<TValue>, RiskOfOptions.Options.BaseOption> modSettingOption = null)
        {
            var binding = this.config.Bind(this.Section, key, description: description,
                defaultValue: defaultValue);
            set(binding.Value);
            this.Bindings.Add(binding);
            this.UpdateCallbacks.Add(binding, () => set(binding.Value));
            RegisterModSetting(modSettingOption, binding);

            return this;
        }

        private static void RegisterModSetting<TValue>(Func<ConfigEntry<TValue>, BaseOption> modSettingOption, ConfigEntry<TValue> binding)
        {
            try
            {
                if (modSettingOption != null)
                {
                    var option = modSettingOption.Invoke(binding);
                    if (!IsAlreadyRegistered(option))
                    {
                        RiskOfOptions.ModSettingsManager.AddOption(option);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex);
            }
        }

        public BindCollection Bind<TValue>(string key,
            ConfigDescription configDescription,
            Action<TValue> set,
            TValue defaultValue = default)
        {
            var binding = this.config.Bind(this.Section, key, defaultValue, configDescription);
            set(binding.Value);
            this.Bindings.Add(binding);
            this.UpdateCallbacks.Add(binding, () => set(binding.Value));
            return this;
        }
        
        static bool IsAlreadyRegistered(RiskOfOptions.Options.BaseOption option)
        {
#if DEBUG
            var identifier = (PluginInfo.PLUGIN_GUID + "." + option.Category + "." + option.Name + "." + option.OptionTypeName)
                .Replace(" ", "_").ToUpper();
            var optionCollection = typeof(ModSettingsManager)
                .GetField("OptionCollection", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);
            Log.Info("optionCollection ? " + optionCollection);

            var _identifierModGuidMap = (Dictionary<string, string>)typeof(ModSettingsManager).Assembly.GetType("RiskOfOptions.Containers.ModIndexedOptionCollection")
                .GetField("_identifierModGuidMap", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(optionCollection);
            return _identifierModGuidMap.ContainsKey(identifier);
#endif
            return false;
        }
    }
}