using Exiled.API.Enums;
using Exiled.API.Features;
using ScriptedEventsAPI.MethodAPI;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.Plugin;

public class Plugin : Plugin<Config>
{
    public override PluginPriority Priority => PluginPriority.Higher;

    public override void OnEnabled()
    {
        base.OnEnabled();
        MethodIndex.Initalize();
        PlayerVariableIndex.Initalize();
    }
}