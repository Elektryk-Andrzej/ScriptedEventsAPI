using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods.PureMethods;

public class SaveMethod : TextReturningStandardMethod
{
    public override string Name => "Save";
    public override string ReturnDescription => "The provided text.";
    
    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new TextArgument("text")
    ];

    public override void Execute()
    {
        TextReturn = Args.GetText("text");
    }
}