using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.DoorMethods;

public class OpenDoor : StandardMethod
{
    public override string Description => "Opens specified doors.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [       
        new DoorArgument("doors")
    ];
    
    public override void Execute()
    {
        var doors = Args.GetDoors("doors");
        
        foreach (var door in doors)
        {
            door.IsOpen = true;
        }
    }
}