using System;
using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods.PureMethods;

public class GetMethod : TextReturningStandardMethod
{
    public override string Name => "Get";
    public override string ReturnDescription => "The requested properties about the player.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new SinglePlayerArgument("player"),
        new OptionsArgument("property",
            "name",
            new("dpName", "the name set by the server"),
            "role",
            "team")
    ];

    public override void Execute()
    {
        var plr = Args.GetPlayer("player");
        TextReturn = Args.GetOption("property").ToLower() switch
        {
            "name" => plr.Nickname,
            "role" => plr.Role.Type.ToString(),
            "team" => plr.Role.Team.ToString(),
            "dpname" => plr.DisplayNickname,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}