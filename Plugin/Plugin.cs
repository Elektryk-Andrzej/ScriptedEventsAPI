using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using ScriptedEventsAPI.MethodSystem;
using ScriptedEventsAPI.Plugin.ScriptFlags;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.Plugin;

public class Plugin : Plugin<Config>
{
    public override PluginPriority Priority => PluginPriority.Higher;
    public override string Author => "Elektryk_Andrzej";
    public override Version Version => new(0, 1, 0);

    public static readonly List<Script> RunningScripts = [];

    public override void OnEnabled()
    {
        base.OnEnabled();
        MethodIndex.Initalize();
        PlayerVariableIndex.Initalize();
        FileSystem.Initalize();
        ScriptFlagHandler.Initialize();
    }
}