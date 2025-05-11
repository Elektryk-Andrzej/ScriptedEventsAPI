using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.MapMethods.RoomLightMethods;

public class SetLightColorMethod : Method
{
    public override string Description => "Sets the light color for rooms.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new RoomsArgument("rooms"),
        new ColorArgument("color"),
    ];
    
    public override void Execute()
    {
        var rooms = Args.GetRooms("rooms");
        var color = Args.GetColor("color");
        
        rooms.ForEach(room => room.Color = color);
    }
}