using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.BroadcastMethods;

public class BroadcastMethod : Method
{
    public override string Description => "Sends a broadcast to players.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new DurationArgument("duration"),
        new TextArgument("message")
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var duration = Args.GetDuration("duration");
        var message = Args.GetText("message");

        foreach (var player in players) player.Broadcast((ushort)duration.Seconds, message);
    }
}