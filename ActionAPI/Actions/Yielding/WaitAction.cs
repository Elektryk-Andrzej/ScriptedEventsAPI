using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using ScriptedEventsAPI.ActionAPI.ActionArguments;
using ScriptedEventsAPI.ActionAPI.Actions.BaseActions;

namespace ScriptedEventsAPI.ActionAPI.Actions.Yielding;

public class WaitAction : YieldingAction
{
    public override string Name => "Wait";
    public override string Description => "Waits for a specified amount of time.";

    public override BaseActionArgument[] RequiredArguments { get; } =
    [
        new TimeSpanArgument("duration")
    ];
    
    public override IEnumerator<float> Execute()
    {
        var dur = ArgsProvided.GetOptional<TimeSpanArgument>("duration");
        Log.Warn($"Value: {dur.Value.TotalSeconds}");
        yield return Timing.WaitForSeconds((float)dur.Value.TotalSeconds);
    }
}