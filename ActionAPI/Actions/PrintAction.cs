using Exiled.API.Features;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.ActionExceptions;
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
        var text = Args.GetText("text");
        Log.Info($"[Script '{Script.Name}'] {text}");
    }
}