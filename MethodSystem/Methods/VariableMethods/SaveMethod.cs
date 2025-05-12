using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.VariableMethods;

public class SaveMethod : TextReturningMethod
{
    public override string Description => "Returns the provided text, which can be saved to a variable.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new TextArgument("text")
    ];

    public override void Execute()
    {
        TextReturn = Args.GetText("text");
    }
}