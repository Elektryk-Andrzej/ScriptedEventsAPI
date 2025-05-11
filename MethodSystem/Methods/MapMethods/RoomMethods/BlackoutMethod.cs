using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.MapMethods.RoomMethods;

public class BlackoutMethod : Method
{
    public override string Description => "Blackouts rooms.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new RoomsArgument("rooms"),
        new DurationArgument("duration")
    ];
    
    public override void Execute()
    {
        var rooms = Args.GetRooms("rooms");
        var duration = Args.GetDuration("duration");
        
        rooms.ForEach(room => room.Blackout(duration.ToFloatSeconds()));
    }
}