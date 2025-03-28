﻿using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
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

    public override IActionResponse Execute()
    {
        Exiled.API.Features.Log.Info(
            $"[Script: {Script.Name}] {Args.Get<TextArgument>("text")}");
        return new SuccessResponse();
    }
}