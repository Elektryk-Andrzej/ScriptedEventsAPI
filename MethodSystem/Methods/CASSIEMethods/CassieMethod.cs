using Exiled.API.Features;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.CASSIEMethods;

public class CassieMethod : Method
{
    public override string Description => "Makes a CASSIE announcement.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new OptionsArgument("mode",
            "loud",
            "silent"),
        new TextArgument("message"),
        new TextArgument("translation")
        {
            DefaultValue = "",
        }
    ];
    
    public override void Execute()
    {
        var isNoisy = Args.GetOption("mode") == "loud";
        var message = Args.GetText("message");
        var translation = Args.GetText("translation");

        Cassie.MessageTranslated(message, translation, false, isNoisy);
    }
}