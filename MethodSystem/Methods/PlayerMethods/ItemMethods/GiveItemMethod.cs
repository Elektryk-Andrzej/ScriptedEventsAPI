using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods.ItemMethods;

public class GiveItemMethod : Method
{
    public override string Description => "Gives an item to players.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new EnumArgument<ItemType>("item"),
        new IntAmountArgument("amount", 1)
        {
            DefaultValue = 1
        }
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var item = Args.GetEnum<ItemType>("item");
        var amount = Args.GetIntAmount("amount");

        foreach (var plr in players) plr.AddItem(item, amount);
    }
}