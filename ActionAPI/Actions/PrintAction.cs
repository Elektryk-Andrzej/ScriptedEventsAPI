using System;
using ScriptedEventsAPI.ActionAPI.ActionArguments;
using ScriptedEventsAPI.ActionAPI.ActionResponses;

namespace ScriptedEventsAPI.ActionAPI.Actions;

public class PrintAction : BaseAction
{
    public override string Name => "Print";
    public override string Description => "Prints the text provided to the server console.";

    public override BaseActionArgument[] RequiredArguments { get; } = [
        new StringArgument("text")
    ];
    
    public override IActionResponse Execute()
    {
        var text = ArgumentsProvided.Get<StringArgument>("text");
        Console.WriteLine(text);
        return new SuccessResponse();
    }
}