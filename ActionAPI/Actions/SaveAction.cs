using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.ActionResponses;
using ScriptedEventsAPI.ActionAPI.BaseActions;

namespace ScriptedEventsAPI.ActionAPI.Actions;

public class SaveAction : StringReturningStandardAction
{
    public override string Name => "Save";
    public override string Description => "Saves the provided text to a variable";

    public override BaseActionArgument[] ExpectedArguments =>
    [
        new TextArgument("text")
    ];
    
    public override IActionResponse Execute()
    {
        Result = Args.Get<TextArgument>("text").ToString();
        return new SuccessResponse();
    }
}