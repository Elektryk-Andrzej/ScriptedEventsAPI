using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods.HealthMethods;

public class DamageMethod : Method
{
    public override string Description => "Damages players.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new FloatAmountArgument("amount", 0),
        new EnumArgument<DamageType>("damageType"),
        new SinglePlayerArgument("attacker")
        {
            IsOptional = true
        }
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var amount = Args.GetFloatAmount("amount");
        var damageType = Args.GetEnum<DamageType>("damageType");
        var attacker = Args.GetSinglePlayer("attacker");

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        Action<Player> hurtAction = attacker is not null
            ? plr => plr.Hurt(attacker, amount, damageType, null!)
            : plr => plr.Hurt(amount, damageType);
        
        foreach (var plr in players)
        {
            hurtAction(plr);
        }
    }
}