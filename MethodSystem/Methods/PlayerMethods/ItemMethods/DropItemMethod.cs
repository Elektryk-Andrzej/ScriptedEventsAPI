using System.Linq;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods.ItemMethods;

public class DropItemMethod : Method
{
    public override string Description => "Drops items from players' inventories.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new EnumArgument<ItemType>("itemTypeToDrop"),
        new IntAmountArgument("amountToDrop", 1)
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var itemTypeToDrop = Args.GetEnum<ItemType>("itemTypeToDrop");
        var amountToDrop = Args.GetIntAmount("amountToDrop");

        foreach (var plr in players)
        {
            var items = plr.Items.Where(item => item.Type == itemTypeToDrop).Take(amountToDrop).ToList();
            foreach (var item in items) plr.DropItem(item);
        }
    }
}