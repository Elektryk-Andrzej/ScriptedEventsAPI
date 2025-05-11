using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.MapMethods.DoorMethods;

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