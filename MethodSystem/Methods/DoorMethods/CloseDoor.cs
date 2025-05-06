using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.DoorMethods;

public class CloseDoors : StandardMethod
{
    public override string Description => "Closes specified doors.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [       
        new DoorArgument("doors")
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