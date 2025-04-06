using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods;

public class BroadcastMethod : StandardMethod
{
    public override string Name => "Broadcast";
    public override string Description => "Sends a broadcast to specified players.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayerVariableArgument("players"),
        new DurationArgument("duration"),
        new TextArgument("message"),
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var duration = Args.GetDuration("duration");
        var message = Args.GetText("message");
        
        foreach (var player in players)
        {
            player.Broadcast((ushort)duration.Seconds, message);
        }
    }
}