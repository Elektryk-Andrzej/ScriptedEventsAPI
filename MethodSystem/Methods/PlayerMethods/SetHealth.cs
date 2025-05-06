using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods;

public class SetHealth : StandardMethod
{
    public override string Description => "Sets the health of specified players.";
    public override BaseMethodArgument[] ExpectedArguments => 
    [
        new PlayersArgument("players"),
        new NumberArgument("health")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var health = Args.GetNumber("health");
        foreach (var player in players) player.Health = health;
    }
}