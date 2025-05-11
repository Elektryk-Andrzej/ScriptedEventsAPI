using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods;

public class ExplodeMethod : Method
{
    public override string Description => "Explodes players.";
    
    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players")
    ];

    public override void Execute()
    {
        foreach (var player in Args.GetPlayers("players")) 
            player.Explode();
    }
}