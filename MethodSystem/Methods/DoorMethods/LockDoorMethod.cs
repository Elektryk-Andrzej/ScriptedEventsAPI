using Exiled.API.Enums;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.MapMethods.DoorMethods;

public class LockDoorMethod : Method
{
    public override string Description => "Locks doors.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [       
        new DoorsArgument("doors"),
        new EnumArgument<DoorLockType>("lockType") 
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