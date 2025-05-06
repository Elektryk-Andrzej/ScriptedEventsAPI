using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods;

public class AmountOf : TextReturningStandardMethod
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