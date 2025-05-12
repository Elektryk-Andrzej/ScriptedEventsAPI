using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class KickMethod : Method
{
    public override string Description => "Kicks players from the server.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new TextArgument("reason")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var reason = Args.GetText("reason");
        
        players.ForEach(p => p.Kick(reason));
    }
}