using Exiled.API.Features;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.BaseActions;

namespace ScriptedEventsAPI.ActionAPI.Actions;

public class TestPlayerVarAction : StandardAction
{
    public override string Name => "TestPlayerVar";
    public override string Description => "";

    public override BaseActionArgument[] ExpectedArguments { get; } =
    [
        new PlayerVariableArgument("players")
    ];
        
    public override void Execute()
    {
        Log.Warn($"Players in variable: {Args.GetPlayers("players").Count}");
    }
}