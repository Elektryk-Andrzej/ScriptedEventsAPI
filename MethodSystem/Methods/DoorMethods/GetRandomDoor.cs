using Exiled.API.Extensions;
using Exiled.API.Features.Doors;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodSystem.Methods.DoorMethods;

public class GetRandomDoor : TextReturningStandardMethod
{
    public override string Description => "Returns a reference to a random door.";
    
    public override BaseMethodArgument[] ExpectedArguments => [];
    
    public override void Execute()
    {
        TextReturn = ObjectReferenceSystem.RegisterObject(Door.List.GetRandomValue());
    }
}