using Exiled.API.Enums;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.DoorMethods;

public class LockDoor : StandardMethod
{
    public override string Description => "Locks specified doors.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [       
        new DoorArgument("doors"),
        new EnumArgument("lockType", typeof(DoorLockType)) 
    ];
    
    public override void Execute()
    {
        var doors = Args.GetDoors("doors");
        var lockType = Args.GetEnum<DoorLockType>("lockType");
        
        foreach (var door in doors)
        {
            door.Lock(lockType);
        }
    }
}