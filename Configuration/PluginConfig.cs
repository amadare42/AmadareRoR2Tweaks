using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AmadareTweaks.Logging;
using BepInEx.Configuration;
using R2API.Utils;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using UnityEngine;

namespace AmadareTweaks.Configuration;

public class PluginConfig
{
    public bool DisplayOtherPlayerItemCount { get; set; }
    public bool DisplayOwnItemCount { get; set; }

    public float PayBeadsChance { get; set; }

    public bool PreventArtifactKeyCommandReplacement { get; set; }

    public bool ChatLogging { get; set; }


    public static PluginConfig Load(ConfigFile config)
    {
        var pluginConfig = new PluginConfig();

        config
            .BindCollection("Other players item count")
            .Bind("Display item count",
                "(Client) When hovering onto item, displays how many instances of it other players have",
                v => pluginConfig.DisplayOtherPlayerItemCount = v,
                true,
                b => new CheckBoxOption(b)
            )
            .Bind("Display own",
                "(Client) If enabled, your own item count will be displayed in list as well. Otherwise it will be filtered out.",
                v => pluginConfig.DisplayOwnItemCount = v,
                true,
                b => new CheckBoxOption(b)
            );

        config.BindCollection("Tweaks")
            .Bind("Prioritize Beads",
                "(Server) Determine chance that Beads of Fealty would be prioritized in Cleansing Pools. Value is chance per one Bead that it would be consumed instead of other items. If 0 - disabled.",
                v => pluginConfig.PayBeadsChance = Mathf.Clamp(v, 0, 100) / 100f,
                defaultValue: 40f,
                b => new SliderOption(b, new SliderConfig { min = 0, max = 100 }))
            .Bind("Prevent Artifact Key Command Replacement",
                "(Server) If enable, will prevent Artifact Key in Artifact Reliquary to be replaced with command substance.",
                v => pluginConfig.PreventArtifactKeyCommandReplacement = v,
                defaultValue: true,
                b => new CheckBoxOption(b));

        config.BindCollection("Debug")
            .Bind("Chat logging", 
                "", 
                v => pluginConfig.ChatLogging = v, false,
                b => new CheckBoxOption(b));

        return pluginConfig;
    }
}