using System;
using Exiled.API.Features.Doors;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.MapMethods.DoorMethods;

public class DoorInfoMethod : TextReturningMethod
{
    public override string Description => "Returns information about the door.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new ReferenceArgument<Door>("door"),
        new OptionsArgument("info",
            "type",
            "isOpen",
            "isClosed",
            "isMoving",
            "isLocked",
            "isUnlocked",
            "name",
            "room")
    ];
    
    public override void Execute()
    {
        var door = Args.GetReference<Door>("door");
        var info = Args.GetOption("info");
        
        TextReturn = info.ToLower() switch
        {
            "type" => door.Type.ToString(),
            "isopen" => door.IsFullyOpen.ToString(),
            "isclosed" => door.IsFullyClosed.ToString(),
            "ismoving" => door.IsMoving.ToString(),
            "islocked" => door.IsLocked.ToString(),
            "isunlocked" => (!door.IsLocked).ToString(),
            "name" => door.Name,
            "room" => door.Room?.Name ?? "None",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}