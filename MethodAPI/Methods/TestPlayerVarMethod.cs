using Exiled.API.Features;
using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods;

public class TestPlayerVarMethod : StandardMethod
{
    public override string Name => "TestPlayerVar";
    public override string Description => "";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayerVariableArgument("players")
    ];
        
    public override void Execute()
    {
        Log.Warn($"Players in variable: {Args.GetPlayers("players").Count}");
    }
}