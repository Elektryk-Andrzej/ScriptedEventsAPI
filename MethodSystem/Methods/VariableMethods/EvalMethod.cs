using NCalc;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.VariableMethods;

public class EvalMethod : TextReturningMethod
{
    public override string Description => "Evaluates the provided expression and returns the result. " +
                                          "Used for math operations.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new TextArgument("value")
    ];

    public override void Execute()
    {
        var value = Args.GetText("value");

        TextReturn = new Expression(value).Evaluate()?.ToString() ?? "UNDEFINED";
    }
}