using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods.PureMethods;

public class PlayerAmountMethod : TextReturningStandardMethod
{
    public override string Name => "PlayerAmount";
    public override string Description => "Returns the amount of players in a given player variable.";
    public override string ReturnDescription => "Amount of players.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayerVariableArgument("variable")
    ];

    public override void Execute()
    {
        TextReturn = Args.GetPlayers("variable").Count.ToString();
    }
}