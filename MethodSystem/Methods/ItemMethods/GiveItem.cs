using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.ItemMethods;

public class GiveItem : StandardMethod
{
    public override string Description => "Gives an item to specified players.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new EnumArgument("item", typeof(ItemType)),
        new AmountArgument("amount", 1)
        {
            DefaultValue = 1
        }
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var item = Args.GetEnum<ItemType>("item");
        var amount = Args.GetAmount("amount");

        foreach (var plr in players) plr.AddItem(item, amount);
    }
}