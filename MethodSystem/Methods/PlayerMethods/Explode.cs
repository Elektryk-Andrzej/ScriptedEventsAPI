using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods;

public class Explode : StandardMethod
{
    public override string Description => "Explodes given players.";
    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players")
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        foreach (var player in players) player.Explode();
    }
}