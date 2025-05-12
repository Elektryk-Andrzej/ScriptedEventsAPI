using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

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