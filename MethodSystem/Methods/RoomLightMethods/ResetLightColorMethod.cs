using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.MapMethods.RoomLightMethods;

public class ResetLightColorMethod : Method
{
    public override string Description => "Resets the light color for rooms.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new RoomsArgument("rooms")
    ];
    
    public override void Execute()
    {
        Args.GetRooms("rooms").ForEach(room => room.ResetColor());
    }
}