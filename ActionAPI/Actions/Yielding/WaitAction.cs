﻿using System.Collections.Generic;
using MEC;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.BaseActions;

namespace ScriptedEventsAPI.ActionAPI.Actions.Yielding;

public class WaitAction : YieldingAction
{
    public override string Name => "Wait";
    public override string Description => "Waits for a specified amount of time.";

    public override BaseActionArgument[] ExpectedArguments { get; } =
    [
        new DurationArgument("duration")
    ];
    
    public override IEnumerator<float> Execute()
    {
        var dur = Args.GetDuration("duration");
        yield return Timing.WaitForSeconds((float)dur.TotalSeconds);
    }
}