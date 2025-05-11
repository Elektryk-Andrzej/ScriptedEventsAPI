using System;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods;

public class GetMethod : TextReturningMethod
{
    public override string Description => "Returns the requested properties about the player.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new SinglePlayerArgument("player"),
        new OptionsArgument("property",
            "name",
            "displayName",
            "role",
            "team")
    ];

    public override void Execute()
    {
        var plr = Args.GetSinglePlayer("player");
        TextReturn = Args.GetOption("property").ToLower() switch
        {
            "name" => plr.Nickname,
            "role" => plr.Role.Type.ToString(),
            "team" => plr.Role.Team.ToString(),
            "displayName" => plr.DisplayNickname,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}