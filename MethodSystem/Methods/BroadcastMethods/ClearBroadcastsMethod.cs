using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.BroadcastMethods;

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