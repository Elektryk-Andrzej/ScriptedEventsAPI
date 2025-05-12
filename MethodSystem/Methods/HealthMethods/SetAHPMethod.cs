using System;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods.HealthMethods;

// ReSharper disable once InconsistentNaming
public class SetAHPMethod : Method
{
    public override string Description => "Sets the amount of AHP for players.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new FloatAmountArgument("amount", 1)
        {
            Description = "The amount of artificial health to ADD to the player. Use a negative number to remove."
        },
        new FloatAmountArgument("limit", 1)
        {
            DefaultValue = 75,
            Description = "The upper limit of AHP."
        },
        new FloatAmountArgument("decay", 0)
        {
            DefaultValue = 1.2,
            Description = "How much AHP is lost per second."
        },
        new PercentageArgument("efficacy")
        {
            DefaultValue = 0.7,
            Description = "The percent of incoming damage absorbed by AHP."
        },
        new DurationArgument("sustain")
        {
            DefaultValue = TimeSpan.Zero,
            Description = "The amount of time before the AHP begins to decay."
        },
        new BoolArgument("isPersistent")
        {
            DefaultValue = false,
            Description = "Whether or not the AHP process stays after AHP reaches 0."
        }
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var amount = Args.GetFloatAmount("amount");
        var limit = Args.GetFloatAmount("limit");
        var decay = Args.GetFloatAmount("decay");
        var efficacy = Args.GetPercentage("efficacy");
        var sustain = Args.GetDuration("sustain");
        var isPersistent = Args.GetBool("isPersistent");

        foreach (var plr in players)
        {
            plr.AddAhp(amount, limit, decay, efficacy, (float)sustain.TotalSeconds, isPersistent);
        }
    }
}