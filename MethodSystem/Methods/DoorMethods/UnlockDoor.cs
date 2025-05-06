using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.DoorMethods;

public class UnlockDoor : StandardMethod
{
    public override string Description => "Unlocks specified doors.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new DoorArgument("doors")
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