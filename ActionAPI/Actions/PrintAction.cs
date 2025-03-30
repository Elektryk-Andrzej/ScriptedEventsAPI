using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;
using ScriptedEventsAPI.ActionAPI.ActionResponses;
using ScriptedEventsAPI.ActionAPI.BaseActions;

namespace ScriptedEventsAPI.ActionAPI.Actions;

public class PrintAction : StandardAction
{
    public override string Name => "Print";
    public override string Description => "Prints the text provided to the server console.";

    public override BaseActionArgument[] ExpectedArguments =>
    [
        new TextArgument("text")
    ];

    public override void Execute()
    {
        Exiled.API.Features.Log.Info(
            $"[Script: {Script.Name}] {Args.GetText("text")}");
        Response = new SuccessResponse();
    }
}