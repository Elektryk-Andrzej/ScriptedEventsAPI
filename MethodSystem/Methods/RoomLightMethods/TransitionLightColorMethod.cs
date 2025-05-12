using System.Collections.Generic;
using UnityEngine;
using Exiled.API.Features;
using MEC;
using SER.Helpers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.MapMethods.RoomLightMethods;

public class TransitionLightColorMethod : Method
{
    public override string Description => "Transitions smoothly the light color for rooms.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new RoomsArgument("rooms"),
        new ColorArgument("color"),
        new DurationArgument("transitionDuration")
    ];
    
    public override void Execute()
    {
        var rooms = Args.GetRooms("rooms");
        var targetColor = Args.GetColor("color");
        var transitionDuration = Args.GetDuration("transitionDuration");
        
        foreach (var room in rooms)
        {
            Timing.RunCoroutine(TransitionColor(room, targetColor, transitionDuration.ToFloatSeconds()));
        }
    }
    
    private static IEnumerator<float> TransitionColor(Room room, Color targetColor, float duration)
    {
        if (room == null)
            yield break;
        
        Color startColor = room.Color;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            
            Color currentColor = Color.Lerp(startColor, targetColor, t);
            room.Color = currentColor;
            
            yield return Timing.WaitForOneFrame;
        }
        
        room.Color = targetColor;
    }
}