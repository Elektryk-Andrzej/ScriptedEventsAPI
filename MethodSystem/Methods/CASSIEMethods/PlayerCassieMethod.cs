using Exiled.API.Extensions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.CASSIEMethods;

public class PlayerCassieMethod : Method
{
    public override string Description => "Makes a CASSIE announcement to specified players only.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new OptionsArgument("mode",
            "jingle",
            "silent"),
        new TextArgument("message"),
        new TextArgument("translation")
        {
            DefaultValue = "",
        }
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var isNoisy = Args.GetOption("mode") == "jingle";
        var message = Args.GetText("message");
        var translation = Args.GetText("translation");

        foreach (var player in players)
        {
            player.MessageTranslated(message, translation, false, isNoisy);
        }
    }
}