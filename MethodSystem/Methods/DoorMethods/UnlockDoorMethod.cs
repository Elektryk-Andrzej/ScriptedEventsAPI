using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.MapMethods.DoorMethods;

public class UnlockDoorMethod : Method
{
    public override string Description => "Unlocks doors.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new DoorsArgument("doors")
    ];

    public override void Execute()
    {
        var doors = Args.GetDoors("doors");

        foreach (var door in doors)
        {
            door.Unlock();
        }
    }
}