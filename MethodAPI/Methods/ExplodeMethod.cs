using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods;

public class ExplodeMethod : StandardMethod
{
    public override string Name => "Explode";
    public override string Description => "Explodes given players.";
    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayerVariableArgument("players")
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        foreach (var player in players) player.Explode();
    }
}