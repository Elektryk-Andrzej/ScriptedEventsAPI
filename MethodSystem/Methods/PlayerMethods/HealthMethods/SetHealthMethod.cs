using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods.HealthMethods;

public class SetHealthMethod : Method
{
    public override string Description => "Sets health for players.";
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