using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class AmountOfMethod : TextReturningMethod
{
    public override string Description => "Returns the amount of players in a given player variable.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("variable")
    ];

    public override void Execute()
    {
        TextReturn = Args.GetPlayers("variable").Count.ToString();
    }
}