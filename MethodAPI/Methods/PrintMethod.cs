using Exiled.API.Features;
using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods;

public class PrintMethod : StandardMethod
{
    public override string Name => "Print";
    public override string Description => "Prints the text provided to the server console.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new TextArgument("text")
    ];

    public override void Execute()
    {
        var text = Args.GetText("text");
        Log.Info($"[Script '{Script.Name}'] {text}");
    }
}