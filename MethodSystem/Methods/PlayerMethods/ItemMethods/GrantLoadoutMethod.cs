using Exiled.API.Extensions;
using PlayerRoles;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods.ItemMethods;

public class GrantLoadoutMethod : Method
{
    public override string Description => "Grants players a class loadout.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new EnumArgument<RoleTypeId>("roleLoadout")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var roleLoadout = Args.GetEnum<RoleTypeId>("roleLoadout");
        var inventory = roleLoadout.GetStartingInventory();
        var ammo = roleLoadout.GetStartingAmmo();

        foreach (var player in players)
        {
            player.AddItem(inventory);
            player.AddAmmo(ammo);
        }
    }
}