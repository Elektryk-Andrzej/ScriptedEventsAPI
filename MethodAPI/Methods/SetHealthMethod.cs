using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods;

public class SetHealthMethod : StandardMethod
{
    public override string Name => "SetHealth";
    public override string Description => "Sets the health of specified players.";
    public override BaseMethodArgument[] ExpectedArguments => 
    [
        new PlayerVariableArgument("players"),
        new NumberArgument("health")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var health = Args.GetNumber("health");
        foreach (var player in players) player.Health = health;
    }
}