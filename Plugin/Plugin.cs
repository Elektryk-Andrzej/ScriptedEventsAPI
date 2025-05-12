using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using SER.MethodSystem;
using SER.ScriptSystem;
using SER.ScriptSystem.ScriptFlags;
using SER.VariableSystem;

namespace SER.Plugin;

public class Plugin : Plugin<Config>
{
    public override PluginPriority Priority => PluginPriority.Higher;
    public override string Author => "Elektryk_Andrzej";
    public override Version Version => new(0, 1, 0);

    public static readonly List<Script> RunningScripts = [];

    public override void OnEnabled()
    {
        base.OnEnabled();
        ExiledEventSystem.Initialize();
        ScriptFlagHandler.Initialize();
        MethodIndex.Initalize();
        PlayerVariableIndex.Initalize();
        FileSystem.Initalize();
    }
}