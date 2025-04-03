using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;
using ScriptedEventsAPI.MethodAPI.Methods.AdditionalDescriptors;

namespace ScriptedEventsAPI.MethodAPI.Methods;

public class SaveMethod : TextReturningStandardMethod, IPureMethod
{
    public override string Name => "Save";
    public override string Description => "Saves the provided text to a variable.";
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