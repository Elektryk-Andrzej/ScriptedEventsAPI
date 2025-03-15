using System;
using ScriptedEventsAPI.ActionAPI.ActionArguments;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;
using ScriptedEventsAPI.ActionAPI.ActionResponses;
using ScriptedEventsAPI.ActionAPI.Actions.BaseActions;
using ScriptedEventsAPI.Other;

namespace ScriptedEventsAPI.ActionAPI.Actions;

public class PrintAction : StandardAction
{
    public override string Name => "Print";
    public override string Description => "Prints the text provided to the server console.";

    public override BaseActionArgument[] RequiredArguments { get; } = 
    [
        new StringArgument("text")
    ];

    public override IActionResponse Execute()
    {
        var text = ArgsProvided.Get<StringArgument>("text");
        Exiled.API.Features.Log.Error(text.Value);
        return new SuccessResponse();
    }
}