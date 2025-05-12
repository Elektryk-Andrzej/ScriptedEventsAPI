using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods.ItemMethods;

public class ClearInventoryMethod : Method
{
    public override string Description => "Clears inventory for players.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players")
    ];
    
    public override void Execute()
    {
        foreach (var plr in Args.GetPlayers("players"))
        {
            plr.Inventory.UserInventory.ReserveAmmo.Clear();
            plr.Inventory.SendAmmoNextFrame = true;
            plr.ClearInventory();
        }
    }
}