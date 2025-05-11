using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.BroadcastMethods;

public class HintMethod : Method
{
    public override string Description => "Sends a hint to players.";

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

        foreach (var player in players)
        {
            player.ShowHint(message, (ushort)duration.Seconds);
        }
    }
}