using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.BroadcastMethods;

public class ClearBroadcastsMethod : Method
{
    public override string Description => "Clears broadcasts for players.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");

        foreach (var plr in players)
        {
            plr.ClearBroadcasts();
        }
    }
}