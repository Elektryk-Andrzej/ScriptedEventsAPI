using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.MapMethods.DoorMethods;

public class CloseDoorMethod : Method
{
    public override string Description => "Closes doors.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [       
        new DoorsArgument("doors")
    ];
    
    public override void Execute()
    {
        var doors = Args.GetDoors("doors");;
        
        foreach (var door in doors)
        {
            door.IsOpen = false;
        }
    }
}