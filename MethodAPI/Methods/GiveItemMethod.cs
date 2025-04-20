using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.Arguments.Structures;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods;

public class GiveItemMethod : StandardMethod
{
    public override string Name => "GiveItem";
    public override string Description => "Gives an item to specified players.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayerVariableArgument("players"),
        new EnumArgument("item", typeof(ItemType)),
        new AmountArgument("amount", 1)
        {
            RequiredInfo = RequiredArgumentInfo.NotRequired(1)
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