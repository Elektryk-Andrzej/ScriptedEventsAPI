using Exiled.API.Enums;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods.HealthMethods;

public class KillMethod : Method
{
    public override string Description => "Kills players.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new EnumArgument<DamageType>("damageType")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var damageType = Args.GetEnum<DamageType>("damageType");
        
        foreach (var player in players)
        {
            player.Kill(damageType);
        }
    }
}