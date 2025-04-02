using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.ActionExceptions;
using ScriptedEventsAPI.ActionAPI.AdditionalActionDescriptors;
using ScriptedEventsAPI.ActionAPI.BaseActions;

namespace ScriptedEventsAPI.ActionAPI.Actions;

public class SaveAction : TextReturningStandardAction, IPureAction
{
    public override string Name => "Save";
    public override string Description => "Saves the provided text to a variable.";
    public override string ReturnDescription => "The provided text.";
    
    public override BaseActionArgument[] ExpectedArguments =>
    [
        new TextArgument("text")
    ];

    public override void Execute()
    {
        TextReturn = Args.GetText("text");
    }
}