using System;
using Discord;
using Exiled.API.Features;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.BackendMethods;

public class PrintMethod : Method
{
    public override string Description => "Prints the text provided to the server console.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new TextArgument("text")
    ];

    public override void Execute()
    {
        var text = Args.GetText("text");
        Log.Send($"[Script '{Script.Name}'] {text}", LogLevel.Info, ConsoleColor.Cyan);
    }
}