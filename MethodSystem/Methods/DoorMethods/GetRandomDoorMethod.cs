using System;
using Exiled.API.Extensions;
using Exiled.API.Features.Doors;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.MapMethods.DoorMethods;

public class GetRandomDoorMethod : ReferenceReturningMethod
{
    public override string Description => "Returns a reference to a random door.";
    
    public override Type ReturnType => typeof(Door);

    public override BaseMethodArgument[] ExpectedArguments => [];
    
    public override void Execute()
    {
        ValueReturn = Door.List.GetRandomValue();
    }
}