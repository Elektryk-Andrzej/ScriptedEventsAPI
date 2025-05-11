using InventorySystem.Items.Usables.Scp330;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods.ItemMethods;

public class GiveCandyMethod : Method
{
    public override string Description => "Gives candy to players.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new EnumArgument<CandyKindID>("candyType"),
        new IntAmountArgument("amount", 1)
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var candyType = Args.GetEnum<CandyKindID>("candyType");
        var amount = Args.GetIntAmount("amount");

        foreach (var plr in players)
        {
            for (int i = 0; i < amount; i++)
            {
                plr.TryAddCandy(candyType);
            }
        }
    }
}